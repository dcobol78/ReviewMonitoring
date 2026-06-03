using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Wildberries.Models;
using System.Net;
using System.Reflection.PortableExecutable;

namespace ReviewMonitoring.Ingestion.Wildberries;

public class WildberriesApiClient
{
    private const string CardBase =
        "https://www.wildberries.ru/__internal/u-card/cards/v4/detail" +
        "?appType=1&curr=rub&dest=123585839&spp=30" +
        "&hide_vflags=4294967296&hide_dtype=15&lang=ru&ab_testing=false&nm=";

    private const string SearchBase =
        "https://www.wildberries.ru/__internal/u-search/exactmatch/ru/common/v18/search" +
        "?appType=1&curr=rub&dest=123585839&resultset=catalog&sort=popular&spp=30" +
        "&lang=ru&locale=ru&hide_dtype=15&hide_vflags=4294967296" +
        "&inheritFilters=false&suppressSpellcheck=false";

    private static readonly string[] FeedbackHosts =
    [
        "https://feedbacks1.wb.ru/feedbacks/v2/",
        "https://feedbacks2.wb.ru/feedbacks/v2/"
    ];

    private readonly IHttpIngestionClient _http;
    private readonly ILogger<WildberriesApiClient> _log;
    private readonly Dictionary<string, string> _headers;

    public WildberriesApiClient(
        IHttpIngestionClient http,
        ILogger<WildberriesApiClient> log,
        string? cookies = null)
    {
        _http = http;
        _log = log;

        _headers = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
        "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            ["Accept"] = "application/json",
            ["Origin"] = "https://www.wildberries.ru",
            ["Referer"] = "https://www.wildberries.ru/"
        };

        if (!string.IsNullOrEmpty(cookies))
            _headers["Cookie"] = cookies;
    }

    public Task<WbCardResponse?> GetCardAsync(long nmId, CancellationToken ct) =>
        _http.GetJsonAsync<WbCardResponse>(CardBase + nmId, ct, _headers);

    // Отзывы по imtId — пробуем оба шарда
    public async Task<WbFeedbacksResponse?> GetFeedbacksAsync(
        long imtId, CancellationToken ct)
    {
        foreach (var host in FeedbackHosts)
        {
            var response = await _http.GetJsonAsync<WbFeedbacksResponse>(
                host + imtId, ct);

            if (response is { FeedbackCount: > 0 })
                return response;
        }

        _log.LogWarning("No feedbacks found for imtId {ImtId}", imtId);
        return null;
    }

    public Task<WbSearchResponse?> GetSearchAsync(
        string query, int page, CancellationToken ct)
    {
        var url = $"{SearchBase}&query={Uri.EscapeDataString(query)}&page={page}";
        return _http.GetJsonAsync<WbSearchResponse>(url, ct, _headers);
    }
}