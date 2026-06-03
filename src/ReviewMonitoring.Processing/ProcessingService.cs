// Processing/ProcessingService.cs
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Processing.Interfaces;

namespace ReviewMonitoring.Processing;

public class ProcessingService : IProcessingService
{
    private readonly IReviewAnalyzer _analyzer;
    private readonly IStatisticsCalculator _stats;
    private readonly ILogger<ProcessingService> _log;

    public ProcessingService(
        IReviewAnalyzer analyzer,
        ILogger<ProcessingService> log)
    {
        _analyzer = analyzer;
        _stats = new StatisticsCalculator();
        _log = log;
    }

    // Уровень 1 — обработка одной площадки (только статистика)
    public Task<SourceResult> ProcessSingleAsync(
        ProcessingSingleRequest request,
        CancellationToken ct)
    {
        _log.LogInformation("Processing source {Provider}, {Count} listings",
            request.ProviderName, request.Listings.Count);

        var listings = request.Listings.ToList();

        var (avgRating, totalReviews, distribution) =
            _stats.CalculateForListings(request.Listings);

        var result = new SourceResult
        {
            Name = request.ProviderName,
            Url = request.OriginalUrl,
            TotalReviews = totalReviews,
            AverageRating = avgRating,
            SourceStatus = SourceStatus.Completed,
            SourceRatingDistribution = distribution,
            Listings = listings.Select(l => l.ListingData).ToList()
        };

        return Task.FromResult(result);
    }

    public async Task<ProcessingResult> ProcessFinalAsync(ProcessingFinalRequest request, CancellationToken ct)
    {
        _log.LogInformation("Final processing, {Count} sources, mode {Mode}",
            request.Sources.Count, request.Mode);

        var aggregate = _stats.CalculateAggregate(request.Sources);

        var result = new ProcessingResult
        {
            Aggregate = aggregate,
            Sources = request.Sources.ToList()
        };

        if (request.Mode == ProcessingMode.LLM && request.ListingReviews.Count > 0)
        {
            try
            {
                var batch = await _analyzer.AnalyzeBatchAsync(
                    new BatchAnalysisRequest
                    {
                        ListingReviews = request.ListingReviews,
                        Sources = request.Sources
                    },
                    ct);

                result.Overall = batch.Overall;

                foreach (var source in result.Sources)
                    foreach (var listing in source.Listings)
                        if (batch.PerListing.TryGetValue(listing.Url, out var analysis))
                            listing.Analysis = analysis;

                if (batch.BestSellerUrl is not null)
                {
                    var best = result.Sources
                        .SelectMany(s => s.Listings)
                        .FirstOrDefault(l => l.Url == batch.BestSellerUrl);

                    if (best is not null)
                        aggregate.BestSeller = best;
                }

                _log.LogInformation(
                    "LLM analysis completed: overall {Pros} pros, {Cons} cons, {Count} listings analyzed",
                    batch.Overall.Pros.Count,
                    batch.Overall.Cons.Count,
                    batch.PerListing.Count);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "LLM analysis failed, returning stats only");
            }
        }

        return result;
    }
}