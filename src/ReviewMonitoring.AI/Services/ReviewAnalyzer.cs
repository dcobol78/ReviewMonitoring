using Microsoft.Extensions.Logging;
using ReviewMonitoring.AI.Client;
using ReviewMonitoring.AI.Models;
using ReviewMonitoring.AI.Prompts;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.AI.Services;
internal class ReviewAnalyzer : IReviewAnalyzer
{
    private readonly IAiClient _ai;
    private readonly AnalysisConfig _analysisConfig;
    private readonly ILogger<ReviewAnalyzer> _log;

    public ReviewAnalyzer(IAiClient ai, AnalysisConfig analysisConfig, ILogger<ReviewAnalyzer> log)
    {
        _ai = ai;
        _analysisConfig = analysisConfig;
        _log = log;
    }

    public async Task<AnalysisSummary> AnalyzeAsync(
        IReadOnlyList<Review> reviews,
        CancellationToken ct)
    {
        if (reviews.Count == 0)
        {
            return new AnalysisSummary
            {
                Summary = "No reviews available"
            };
        }

        // Сэмплинг если отзывов много
        var sampled = reviews.Count > 200
            ? SampleReviews(reviews, 200)
            : reviews;

        var prompt = ReviewAnalysisPrompt.Analyze(sampled);

        var result = await _ai.CompleteStructuredAsync<AnalysisSummary>(prompt, ct);

        if (result is null)
        {
            _log.LogWarning("Failed to analyze reviews");
            return new AnalysisSummary
            {
                Summary = "Analysis failed"
            };
        }

        return result;
    }

    public async Task<BatchAnalysisResult> AnalyzeBatchAsync(
    BatchAnalysisRequest request,
    CancellationToken ct)
    {
        var indexed = new List<(string Index, ListingReviews Listing)>();
        var indexToUrl = new Dictionary<string, string>();

        var i = 1;
        var budget = _analysisConfig.MaxTotalReviews;

        foreach (var lr in request.ListingReviews)
        {
            if (budget <= 0)
                break;

            var index = $"L{i++}";
            indexToUrl[index] = lr.ListingData.Url;

            var limit = Math.Min(_analysisConfig.MaxReviewsPerListing, budget);
            var sampled = SampleReviews(lr.Reviews, limit);
            budget -= sampled.Count;

            indexed.Add((index, new ListingReviews
            {
                ListingData = lr.ListingData,
                Reviews = sampled
            }));
        }

        var result = new BatchAnalysisResult();

        if (indexed.Count == 0)
            return result;

        var prompt = ListingsAnalysisPrompt.AnalyzeBatch(indexed);
        var dto = await _ai.CompleteStructuredAsync<BatchAnalysisDto>(prompt, ct);

        if (dto is null)
        {
            _log.LogWarning("Batch analysis returned null");
            return result;
        }

        // Обратный маппинг index → Url
        foreach (var listing in dto.Listings)
        {
            if (indexToUrl.TryGetValue(listing.Index, out var url))
                result.PerListing[url] = new AnalysisSummary
                {
                    Pros = listing.Pros,
                    Cons = listing.Cons,
                    Keywords = listing.Keywords,
                    Flags = listing.Flags,
                    Summary = listing.Summary
                };
        }

        result.Overall = new AnalysisSummary
        {
            Pros = dto.Overall.Pros,
            Cons = dto.Overall.Cons,
            Keywords = dto.Overall.Keywords,
            Flags = dto.Overall.Flags,
            Summary = dto.Overall.Summary
        };

        if (!string.IsNullOrEmpty(dto.BestSeller) &&
    indexToUrl.TryGetValue(dto.BestSeller, out var bestUrl))
        {
            result.BestSellerUrl = bestUrl;
        }

        _log.LogInformation("Batch analysis: {Count} listings, best seller: {Best}",
            result.PerListing.Count,
            result.BestSellerUrl ?? "не выбран");

        return result;
    }

    private static IReadOnlyList<Review> SampleReviews(IReadOnlyList<Review> reviews, int targetCount)
    {
        if (reviews.Count == 0 || targetCount <= 0)
            return [];

        if (reviews.Count <= targetCount)
            return reviews.ToList();

        var byRating = reviews
            .GroupBy(r => r.Rating)
            .ToDictionary(g => g.Key, g => g.ToList());

        var perRating = Math.Max(1, targetCount / byRating.Count);

        return byRating
            .SelectMany(kvp => kvp.Value
                .OrderByDescending(r => r.Likes)
                .Take(perRating))
            .Take(targetCount)
            .ToList();
    }
}
