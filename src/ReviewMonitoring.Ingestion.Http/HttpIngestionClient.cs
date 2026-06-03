using System.Text.Json;
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Ingestion.Interfaces;

namespace ReviewMonitoring.Ingestion.Http;

internal class HttpIngestionClient : IHttpIngestionClient
{
    private readonly HttpClient _http;
    private readonly ILogger<HttpIngestionClient> _log;

    public HttpIngestionClient(HttpClient http, ILogger<HttpIngestionClient> log)
    {
        _http = http;
        _log = log;
    }

    public async Task<string?> GetAsync(
        string url,
        CancellationToken ct,
        Dictionary<string, string>? headers = null)
    {
        try
        {
            _log.LogInformation("GET {Url}", url);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (headers is not null)
                foreach (var (key, value) in headers)
                    request.Headers.TryAddWithoutValidation(key, value);

            var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed GET {Url}", url);
            return null;
        }
    }

    public async Task<T?> GetJsonAsync<T>(
        string url,
        CancellationToken ct,
        Dictionary<string, string>? headers = null)
    {
        var json = await GetAsync(url, ct, headers);
        if (json is null)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to deserialize response from {Url}", url);
            return default;
        }
    }
}