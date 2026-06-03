using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Models;
using System.Threading.Channels;

namespace ReviewMonitoring.Application.Services;
public class JobProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JobProcessor> _log;

    public JobProcessor(IServiceScopeFactory scopeFactory, ILogger<JobProcessor> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
    }

    public async Task ProcessAsync(Job job, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IJobCacheRepository>();
        var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
        var ingestion = scope.ServiceProvider.GetRequiredService<IIngestionService>();
        var processing = scope.ServiceProvider.GetRequiredService<IProcessingService>();

        try
        {
            job.StartParsing();
            await cache.SetAsync(job);

            var allListings = new List<ListingReviews>();
            var sourses = new List<SourceResult>();
            var channel = Channel.CreateUnbounded<IngestionProgress>();

            var processingTask = Task.Run(async () =>
            {
                await foreach (var source in channel.Reader.ReadAllAsync(ct))
                {
                    var singleResult = await processing.ProcessSingleAsync(
                        new ProcessingSingleRequest
                        {
                            Listings = source.Listings,
                            ProviderName = source.ProviderName,
                            OriginalUrl = source.OriginalUrl
                        }, ct);

                    //потенциально не обязательно
                    sourses.Add(singleResult);

                    job.AddSourceResult(singleResult);
                    allListings.AddRange(source.Listings);
                    job.AddCollectedReviews(source.ReviewsCollected);
                    job.AddCollectedListings(source.ListingsCollected);
                    await cache.SetAsync(job);
                }
            }, ct);

            var progress = new Progress<IngestionProgress>(source =>
                channel.Writer.TryWrite(source));

            //TODO: Передавать конфиг
            await ingestion.IngestAsync(
                new AnalysisRequest 
                { 
                    Query = job.Query, 
                    Mode = job.Mode,
                    Config = job.SearchConfig
                },
                progress,
                ct);

            channel.Writer.Complete();

            // теперь awaits — исключение не проглотится
            await processingTask;

            job.StartAnalyzing();
            await cache.SetAsync(job);

            var result = await processing.ProcessFinalAsync(
                new ProcessingFinalRequest
                {
                    Sources = sourses,
                    ListingReviews = allListings,
                    Mode = job.Mode
                }, ct);

            job.Complete(result);
            await repository.SaveAsync(job);
            await cache.DeleteAsync(job.Id);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Job {JobId} failed", job.Id);

            job.Fail(ex.Message);

            try
            {
                await repository.SaveAsync(job);
                await cache.DeleteAsync(job.Id);
            }
            catch (Exception saveEx)
            {
                _log.LogError(saveEx, "Failed to save failed job {JobId}", job.Id);
            }
        }
    }
}
