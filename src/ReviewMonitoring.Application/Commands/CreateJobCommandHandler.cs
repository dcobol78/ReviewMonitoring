using MediatR;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Models;

//TODO: Добавить обработку ошибок и сами ошибки тоже можжно добавить впринципе
//TODO: Logs

namespace ReviewMonitoring.Application.Commands;
public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Guid>
{
    private readonly IJobCacheRepository _cache;
    private readonly IIngestionService _ingestion;
    private readonly IJobRepository _repository;
    private readonly IProcessingService _processing;


    public CreateJobCommandHandler(IJobCacheRepository cache, 
        IIngestionService ingestion, 
        IJobRepository repository, 
        IProcessingService processing)
    {
        _cache = cache;
        _ingestion = ingestion;
        _repository = repository;
        _processing = processing;
    }

    public async Task<Guid> Handle(CreateJobCommand request, CancellationToken ct)
    {
        var availableProviders = _ingestion.GetEnabledProviders();

        var job = Job.Create(request.query, request.mode, availableProviders);
        
        await _cache.SetAsync(job);

        _ = ProcessJobAsync(job, CancellationToken.None);

        return job.Id;
    }

    //Возможно стоит сделать отдельным воркером
    private async Task ProcessJobAsync(Job job, CancellationToken ct)
    {
        job.StartParsing();
        await _cache.SetAsync(job);

        var allReviews = new List<Review>();

        var progress = new Progress<IngestionProgress>(async source =>
        {
            // обновляем статус источника в Job
            job.AddCollectedReviews(source.ReviewsCollected);

            var singleResult = await _processing.ProcessSingleAsync(
                new ProcessingSingleRequest { Mode = job.Mode, Reviews = source.Reviews, SourceName = source.SourceName},
                ct);

            job.AddSourceResult(singleResult);

            allReviews.AddRange(source.Reviews);

            // клиент видит обновление через стриминг
            await _cache.SetAsync(job);
        });

        await _ingestion.IngestAsync(
            new IngestionRequest { Query = job.Query },
            progress,
            ct);

        job.StartAnalyzing();
        await _cache.SetAsync(job);

        var result = await _processing.ProcessFinalAsync(
            new ProcessingFinalRequest { Reviews = allReviews, Mode = job.Mode },
            ct);

        job.Complete(result);

        await _repository.SaveAsync(job);
        await _cache.DeleteAsync(job.Id);
    }
}