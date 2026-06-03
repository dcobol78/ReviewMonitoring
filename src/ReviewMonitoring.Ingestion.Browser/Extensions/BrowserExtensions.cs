using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Ingestion.Interfaces;

namespace ReviewMonitoring.Ingestion.Browser.Extensions;
public static class BrowserExtensions
{
    public static IServiceCollection AddBrowserIngestion(this IServiceCollection services)
    {
        services.AddSingleton<IBrowserSession, BrowserSession>();
        return services;
    }
}