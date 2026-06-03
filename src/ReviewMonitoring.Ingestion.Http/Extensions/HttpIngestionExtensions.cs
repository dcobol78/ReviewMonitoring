using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Ingestion.Interfaces;
using System.Net;

namespace ReviewMonitoring.Ingestion.Http.Extensions;
public static class HttpIngestionExtensions
{
    public static IServiceCollection AddHttpIngestion(
        this IServiceCollection services)
    {
        services.AddHttpClient<IHttpIngestionClient, HttpIngestionClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip
            | DecompressionMethods.Deflate
            | DecompressionMethods.Brotli
        });

        return services;
    }
}