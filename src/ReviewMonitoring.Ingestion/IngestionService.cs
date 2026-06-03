using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Models;
using ReviewMonitoring.Shared.Helpers;

namespace ReviewMonitoring.Ingestion;

public class IngestionService : IIngestionService
{
    private readonly IEnumerable<IIngestionProvider> _providers;
    private readonly IProductMatcher _productMatcher;
    private readonly ILogger<IngestionService> _log;

    public IngestionService(
        IEnumerable<IIngestionProvider> providers,
        IProductMatcher productMatcher,
        ILogger<IngestionService> log)
    {
        _providers = providers;
        _productMatcher = productMatcher;
        _log = log;
    }

    public IReadOnlyList<string> GetEnabledProviders()
    {
        return _providers.Select(p => p.ProviderName).ToList();
    }

    public async Task<IngestionResult> IngestAsync(
        AnalysisRequest request,
        IProgress<IngestionProgress>? progress,
        CancellationToken ct)
    {
        if (request is null)
        {
            _log.LogError("AnalysisRequest is null");
            return IngestionResult.Fail("Request is null");
        }

        // ищем подходящий для ссылки сервис
        var primaryProvider = _providers.FirstOrDefault(p => p.CanHandle(request.Query));
        if (primaryProvider is null)
        {
            _log.LogWarning("No provider can handle {Query}", request.Query);
            return IngestionResult.Fail("No provider can handle this URL");
        }

        // парсим название
        _log.LogInformation("Fetching title from {Query}", request.Query);
        var primaryTitle = await primaryProvider.FetchTitleAsync(request.Query, ct)
            ?? request.Query;

        _log.LogInformation("Title fetched: {Title}", primaryTitle);

        // извлекаем поисковые термины через LLM
        var searchTerms = await _productMatcher.ExtractSearchTermsAsync(primaryTitle, ct);
        _log.LogInformation("Search terms extracted: {Exact}", searchTerms.Exact);

        var config = request.Config ?? new IngestionConfig();

        // параллельно ищем кандидатов на всех провайдерах
        var searchTasks = _providers.Select(async provider =>
        {
            try
            {
                var candidates = await provider.SearchAsync(searchTerms, ct);
                _log.LogInformation("Provider {Provider} found {Count} candidates",
                    provider.ProviderName, candidates.Count);
                return candidates;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Provider {Provider} search failed", provider.ProviderName);
                return new List<ProductCandidate>();
            }
        });

        var searchResults = await Task.WhenAll(searchTasks);
        var allCandidates = searchResults.SelectMany(c => c).ToList();

        // LLM фильтр
        var matchedCandidates = await _productMatcher.FilterMatchesAsync(
            primaryTitle, allCandidates, config, ct);

        _log.LogInformation("LLM matched {Count} listings", matchedCandidates.Count);

        // добавляем изначальную ссылку на случай если LLM дал сбой
        var normalizedQuery = UrlHelper.NormalizeUrl(request.Query);
        if (!matchedCandidates.Any(c => UrlHelper.NormalizeUrl(c.Url) == normalizedQuery))
        {
            matchedCandidates.Insert(0, new ProductCandidate
            {
                Title = primaryTitle,
                Url = request.Query,
                ProviderName = primaryProvider.ProviderName
            });
        }

        // группируем URL по провайдерам
        var urlsByProvider = matchedCandidates
            .GroupBy(c => c.ProviderName)
            .ToDictionary(
                g => _providers.First(p => p.ProviderName == g.Key),
                g => g.Select(c => c.Url).ToList());

        // параллельно парсим каждый провайдер
        var ingestTasks = urlsByProvider.Select(kvp =>
            ProcessProviderAsync(kvp.Key, kvp.Value, config, progress, ct));

        await Task.WhenAll(ingestTasks);

        return IngestionResult.Ok();
    }

    private async Task ProcessProviderAsync(
        IIngestionProvider provider,
        List<string> urls,
        IngestionConfig config,
        IProgress<IngestionProgress>? progress,
        CancellationToken ct)
    {
        try
        {
            var request = new IngestionRequest
            {
                Urls = urls,
                Config = config
            };

            var result = await provider.IngestAsync(request, ct);
            progress?.Report(result);

            _log.LogInformation("Provider {Provider} ingested {Count} listings",
                provider.ProviderName, result.Listings.Count);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Provider {Provider} ingestion failed", provider.ProviderName);

            progress?.Report(new IngestionProgress
            {
                ProviderName = provider.ProviderName,
                OriginalUrl = urls.FirstOrDefault() ?? string.Empty,
                Status = SourceStatus.Failed,
                Listings = []
            });
        }
    }
}