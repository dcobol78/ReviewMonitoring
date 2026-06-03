using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Models;
using System.Text.RegularExpressions;

namespace ReviewMonitoring.Ingestion.Wildberries;

public class WildberriesIngestionProvider : IIngestionProvider
{
    private readonly WildberriesApiClient _api;
    private readonly ILogger<WildberriesIngestionProvider> _log;

    public WildberriesIngestionProvider(
        IHttpIngestionClient http,
        IConfiguration configuration,
        ILogger<WildberriesIngestionProvider> log,
        ILogger<WildberriesApiClient> apiLog)
    {
        var cookies = configuration["Wildberries:Cookies"];
        _api = new WildberriesApiClient(http, apiLog, cookies);
        _log = log;
    }

    public string ProviderName => "Wildberries";

    public bool CanHandle(string? url) =>
        !string.IsNullOrEmpty(url) && url.Contains("wildberries.ru");

    public async Task<string?> FetchTitleAsync(string url, CancellationToken ct)
    {
        var nmId = ExtractNmId(url);
        if (nmId is null)
            return null;

        var card = await _api.GetCardAsync(nmId.Value, ct);
        return card?.Products.FirstOrDefault()?.Name;
    }

    public async Task<IngestionProgress> IngestAsync(
        IngestionRequest request, CancellationToken ct)
    {
        var listings = new List<ListingReviews>();

        foreach (var url in request.Urls)
        {
            try
            {
                var lr = await ParseListingAsync(url, request.Config, ct);
                if (lr is not null)
                    listings.Add(lr);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to parse WB listing {Url}", url);
            }
        }

        return new IngestionProgress
        {
            ProviderName = ProviderName,
            OriginalUrl = request.Urls.FirstOrDefault() ?? string.Empty,
            Status = listings.Count > 0 ? SourceStatus.Completed : SourceStatus.NotFound,
            Listings = listings
        };
    }

    private async Task<ListingReviews?> ParseListingAsync(
        string url, IngestionConfig config, CancellationToken ct)
    {
        var nmId = ExtractNmId(url);
        if (nmId is null)
            return null;

        var card = await _api.GetCardAsync(nmId.Value, ct);
        var product = card?.Products.FirstOrDefault();
        if (product is null)
        {
            _log.LogWarning("No WB card data for {Url}", url);
            return null;
        }

        var feedbacks = await _api.GetFeedbacksAsync(product.Root, ct);

        var reviews = feedbacks is not null
            ? WildberriesPageParser
                .ParseFeedbacks(feedbacks, nmId.Value, url)
                .Take(config.MaxReviewsPerListing)
                .ToList()
            : [];

        var priceKopecks = product.Sizes.FirstOrDefault()?.Price?.Product ?? 0;

        var listing = new Listing
        {
            Url = WildberriesPageParser.BuildProductUrl(nmId.Value),
            SellerName = product.Supplier,
            ProductTitle = product.Name,
            Price = priceKopecks / 100m,
            Rating = product.ReviewRating,
            ReviewCount = product.Feedbacks,
            ImageUrls = WildberriesPageParser.BuildImageUrls(nmId.Value, 8)
        };

        return new ListingReviews
        {
            ListingData = listing,
            Reviews = reviews
        };
    }

    public async Task<List<ProductCandidate>> SearchAsync(
        ProductSearchTerms terms, CancellationToken ct)
    {
        var candidates = new List<ProductCandidate>();
        var seen = new HashSet<string>();

        var queries = new List<string> { terms.Exact };
        queries.AddRange(terms.Synonyms);

        foreach (var query in queries.Take(2))
        {
            try
            {
                var response = await _api.GetSearchAsync(query, 1, ct);
                if (response is null)
                    continue;

                foreach (var c in WildberriesPageParser.ParseSearch(response))
                    if (seen.Add(c.Url))
                        candidates.Add(c);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "WB search failed for {Query}", query);
            }

            await Task.Delay(Random.Shared.Next(300, 700), ct);
        }

        _log.LogInformation("Found {Count} WB candidates", candidates.Count);
        return candidates;
    }

    private static long? ExtractNmId(string url)
    {
        var match = Regex.Match(url, @"/catalog/(\d+)/");
        return match.Success && long.TryParse(match.Groups[1].Value, out var id)
            ? id
            : null;
    }
}