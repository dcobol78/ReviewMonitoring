using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Interfaces;
using System.Text.Json;

namespace ReviewMonitoring.Ingestion;

public class MockIngestionService : IIngestionService
{
    private readonly IEnumerable<IIngestionProvider> _providers;
    private readonly IProductMatcher _productMatcher;
    private readonly ILogger<MockIngestionService> _log;

    public MockIngestionService(
        IEnumerable<IIngestionProvider> providers,
        IProductMatcher productMatcher,
        ILogger<MockIngestionService> log)
    {
        _providers = providers;
        _productMatcher = productMatcher;
        _log = log;
    }

    public IReadOnlyList<string> GetEnabledProviders()
        => new[] { "Ozon", "Wildberries" };

    public async Task<IngestionResult> IngestAsync(
        AnalysisRequest request,
        IProgress<IngestionProgress>? progress,
        CancellationToken ct)
    {
        _log.LogInformation("MockIngestionService: формирование демонстрационных данных для {Query}", request.Query);

        progress?.Report(new IngestionProgress
        {
            ProviderName = "Ozon",
            OriginalUrl = request.Query,
            Status = SourceStatus.Completed,
            Listings = BuildFakeListings("Ozon"),
        });

        progress?.Report(new IngestionProgress
        {
            ProviderName = "Wildberries",
            OriginalUrl = request.Query,
            Status = SourceStatus.Completed,
            Listings = BuildFakeListings("Wildberries"),
        });

        await Task.Yield();
        await Task.Delay(50, ct);

        return IngestionResult.Ok();
    }

    private static List<ListingReviews> BuildFakeListings(string provider)
    {
        var isOzon = provider == "Ozon";

        var url = isOzon
            ? "https://www.ozon.ru/product/fuze-protein-matrix-chocolate-750g-1700000001/"
            : "https://www.wildberries.ru/catalog/170000002/detail.aspx";

        var listing = new Listing
        {
            ProductTitle = "Протеиновый коктейль FUZE PROTEIN MATRIX Chocolate, 750 г",
            SellerName = isOzon ? "PURE" : "SportNutrition",
            Price = isOzon ? 852m : 2533m,
            Rating = isOzon ? 4.8m : 4.5m,
            ReviewCount = isOzon ? 96 : 41,
            Url = url,
            RatingDistribution = isOzon
                ? new Dictionary<int, int> { [5] = 76, [4] = 12, [3] = 4, [2] = 2, [1] = 2 }
                : new Dictionary<int, int> { [5] = 30, [4] = 7, [3] = 2, [2] = 1, [1] = 1 },
            ImageUrls = new List<string>(),
            Extras = new Dictionary<string, JsonElement>(),
        };

        var reviews = new List<Review>
    {
        new Review
        {
            Id         = Guid.NewGuid().ToString(),
            ListingUrl = url,
            Rating     = 5,
            Text       = "Отличный вкус шоколада, хорошо размешивается. Достоинства: вкус, цена. Недостатки: немного пенится.",
            AuthorName = "Иван П.",
            Likes      = 14,
            Date       = DateTime.UtcNow.AddDays(-12),
        },
        new Review
        {
            Id         = Guid.NewGuid().ToString(),
            ListingUrl = url,
            Rating     = 4,
            Text       = "Даёт энергию, хорошая упаковка. Из минусов — состав чуть слаще, чем в описании.",
            AuthorName = "Мария К.",
            Likes      = 6,
            Date       = DateTime.UtcNow.AddDays(-5),
        },
    };

        return new List<ListingReviews>
    {
        new ListingReviews
        {
            ListingData = listing,
            Reviews = reviews,
        }
    };
    }
}