using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Models;

namespace ReviewMonitoring.Ingestion.Ozon;

public class OzonIngestionProvider : IIngestionProvider
{
    private readonly OzonApiClient _api;
    private readonly ILogger<OzonIngestionProvider> _log;

    private const string UrlMustHave = "ozon.ru";
    private const string OzonBase = "https://www.ozon.ru";

    public string ProviderName => "Ozon";

    public OzonIngestionProvider(IHttpIngestionClient http,
        IConfiguration configuration,
        ILogger<OzonIngestionProvider> log,
        ILogger<OzonApiClient> apiLog)
    {
        var cookies = configuration["Ozon:Cookies"];
        _api = new OzonApiClient(http, apiLog, cookies);       
        _log = log;
    }

    public bool CanHandle(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return false;
        return url.Contains(UrlMustHave, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string?> FetchTitleAsync(string url, CancellationToken ct)
    {
        try
        {
            var page = await _api.GetReviewsPageAsync(url, ct);
            if (page is null)
                return null;

            return OzonPageParser.ParseProductData(page)?.Title;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to fetch title from {Url}", url);
            return null;
        }
    }

    public async Task<List<ProductCandidate>> SearchAsync(
        ProductSearchTerms terms,
        CancellationToken ct)
    {
        var candidates = new List<ProductCandidate>();
        var seen = new HashSet<string>();

        var queries = new List<string> { terms.Exact };
        queries.AddRange(terms.Synonyms);

        foreach (var query in queries.Take(2))
        {
            try
            {
                _log.LogInformation("Searching Ozon for {Query}", query);

                var response = await _api.GetSearchPageAsync(query, ct);
                var pagesLoaded = 0;

                while (response is not null && pagesLoaded < 2)
                {
                    var found = OzonPageParser.ParseSearchResults(response);
                    foreach (var c in found)
                        if (seen.Add(c.Url))
                            candidates.Add(c);

                    var nextPage = OzonPageParser.ParseSearchNextPage(response);
                    if (string.IsNullOrEmpty(nextPage))
                        break;

                    await Task.Delay(Random.Shared.Next(500, 1000), ct);
                    response = await _api.GetSearchNextPageAsync(nextPage, ct);
                    pagesLoaded++;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Ozon search failed for {Query}", query);
            }

            await Task.Delay(Random.Shared.Next(500, 1000), ct);
        }

        _log.LogInformation("Found {Count} candidates on Ozon", candidates.Count);
        return candidates;
    }

    public async Task<IngestionProgress> IngestAsync(
        IngestionRequest request,
        CancellationToken ct)
    {
        var listings = new List<ListingReviews>();
        var urlsToProcess = new List<string>(request.Urls);

        foreach (var url in urlsToProcess)
        {
            try
            {
                var listingReviews = await ParseListingAsync(url, request.Config, ct);

                if (listingReviews is null)
                {
                    _log.LogWarning("Failed to parse listing {Url}", url);
                    continue;
                }

                listings.Add(listingReviews);

                // Добавляем других продавцов в очередь
                if (request.Config.ParseAdditionalSellers &&
                    listingReviews.ListingData.Extras is not null &&
                    listingReviews.ListingData.Extras.TryGetValue("otherSellersUrl", out var modalUrlEl))
                {
                    var otherUrls = await FetchOtherSellersAsync(
                        modalUrlEl.GetString() ?? string.Empty, ct);

                    foreach (var otherUrl in otherUrls)
                        if (!urlsToProcess.Contains(otherUrl))
                            urlsToProcess.Add(otherUrl);
                }

                await Task.Delay(Random.Shared.Next(500, 1500), ct);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to parse listing {Url}", url);
            }
        }

        return new IngestionProgress
        {
            ProviderName = ProviderName,
            OriginalUrl = request.Urls.FirstOrDefault() ?? string.Empty,
            Status = Domain.Enums.SourceStatus.Completed,
            Listings = listings
        };
    }

    private async Task<ListingReviews?> ParseListingAsync(
        string url,
        IngestionConfig config,
        CancellationToken ct)
    {
        _log.LogInformation("Parsing listing {Url}", url);


        var productPage = await _api.GetProductPageAsync(url, ct);
        if (productPage is null)
            return null;

        var productData = OzonPageParser.ParseProductData(productPage);
        if (productData is null)
        {
            _log.LogWarning("No product data at {Url}", url);
            return null;
        }

        var firstPage = await _api.GetReviewsPageAsync(url, ct);
        if (firstPage is null)
            return null;

        // Собираем отзывы постранично
        var allReviews = new List<Review>();
        var (reviews, nextPage) = OzonPageParser.ParseReviewsPage(firstPage, url);
        allReviews.AddRange(reviews);

        _log.LogInformation("Page 1: {Count}/{Total} reviews",
            allReviews.Count, firstPage.Paging?.Total ?? 0);

        while (!string.IsNullOrEmpty(nextPage) &&
               allReviews.Count < config.MaxReviewsPerListing)
        {
            await Task.Delay(Random.Shared.Next(300, 800), ct);

            var nextResponse = await _api.GetReviewsNextPageAsync(nextPage, ct);
            if (nextResponse is null)
                break;

            var (nextReviews, nextNextPage) = OzonPageParser.ParseReviewsPage(
                nextResponse, url);

            allReviews.AddRange(nextReviews);
            nextPage = nextNextPage;

            _log.LogInformation("Collected {Count}/{Total} reviews",
                allReviews.Count, firstPage.Paging?.Total ?? 0);
        }

        _log.LogInformation("Finished {Url} — {Count} reviews collected",
            url, allReviews.Count);

        var listing = new Listing
        {
            Url = url,
            SellerName = productData.SellerName,
            ProductTitle = productData.Title,
            Price = productData.Price,
            Rating = productData.Rating,
            ReviewCount = firstPage.Paging?.Total ?? allReviews.Count,
            ImageUrls = productData.ImageUrls,
            Extras = productData.OtherSellersModalUrl is not null
                ? new Dictionary<string, System.Text.Json.JsonElement>
                {
                    ["otherSellersUrl"] = System.Text.Json.JsonSerializer
                        .SerializeToElement(productData.OtherSellersModalUrl)
                }
                : null
        };

        return new ListingReviews
        {
           ListingData = listing,
           Reviews = allReviews
        };
    }

    private async Task<List<string>> FetchOtherSellersAsync(
        string modalUrl,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(modalUrl))
            return [];

        try
        {
            var fullUrl = OzonBase + modalUrl;
            _log.LogInformation("Fetching other sellers from {Url}", fullUrl);

            var page = await _api.GetRawPageAsync(fullUrl, ct);
            return page is null ? [] : OzonPageParser.ParseOtherSellerUrls(page);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to fetch other sellers");
            return [];
        }
    }
}