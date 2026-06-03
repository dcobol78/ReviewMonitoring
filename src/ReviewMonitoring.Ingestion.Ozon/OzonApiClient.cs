using Microsoft.Extensions.Logging;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Ozon.Consts;
using ReviewMonitoring.Ingestion.Ozon.Models;

public class OzonApiClient
{
    private readonly IHttpIngestionClient _http;
    private readonly ILogger<OzonApiClient> _log;
    private readonly Dictionary<string, string> _headers;

    private const string ApiBase =
        "https://www.ozon.ru/api/entrypoint-api.bx/page/json/v2?url=";

    public OzonApiClient(
        IHttpIngestionClient http,
        ILogger<OzonApiClient> log,
        string? cookies = null)
    {
        _http = http;
        _log = log;
        _headers = ConstsOzon.DefualtHeaders;

        if (!string.IsNullOrEmpty(cookies))
            _headers["Cookie"] = cookies;
    }

    public async Task<OzonPageResponse?> GetSearchPageAsync(
        string query, CancellationToken ct)
    {
        var searchPath = $"/search/?text={Uri.EscapeDataString(query)}&from_global=true";
        var url = ApiBase + Uri.EscapeDataString(searchPath);
        return await _http.GetJsonAsync<OzonPageResponse>(url, ct, _headers);
    }

    public async Task<OzonPageResponse?> GetSearchNextPageAsync(
    string nextPage, CancellationToken ct)
    {
        var url = ApiBase + Uri.EscapeDataString(nextPage);
        return await _http.GetJsonAsync<OzonPageResponse>(url, ct, _headers);
    }

    public async Task<OzonPageResponse?> GetProductPageAsync(
    string productUrl, CancellationToken ct)
    {
        var path = ExtractPath(productUrl);
        return await FetchAsync(path, ct);
    }

    public async Task<OzonPageResponse?> GetReviewsPageAsync(
        string productUrl, CancellationToken ct)
    {
        var path = ExtractPath(productUrl) +
            "?layout_container=reviewshelfpaginator" +
            "&layout_page_index=2" +
            "&page=1" +
            "&sort=published_at_desc";

        return await FetchAsync(path, ct);
    }

    public async Task<OzonPageResponse?> GetReviewsNextPageAsync(
        string nextPage, CancellationToken ct)
        => await FetchAsync(nextPage, ct);

    public async Task<OzonPageResponse?> GetRawPageAsync(
        string url, CancellationToken ct)
        => await _http.GetJsonAsync<OzonPageResponse>(url, ct, _headers);

    private async Task<OzonPageResponse?> FetchAsync(string path, CancellationToken ct)
    {
        var url = ApiBase + Uri.EscapeDataString(path);
        return await _http.GetJsonAsync<OzonPageResponse>(url, ct, _headers);
    }

    private static string ExtractPath(string url)
    {
        var uri = new Uri(url);
        return uri.AbsolutePath.TrimEnd('/') + '/';
    }
}