using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Ingestion.Consts;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Shared.Extensions;
using System.Reflection;

namespace ReviewMonitoring.Ingestion.Extensions;

public static class IngestionExtensions
{
    public static IServiceCollection AddIngestion(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        if (configuration.IsDemoMode())
        {
            services.AddScoped<IIngestionService, MockIngestionService>();
            return services;
        }

        var enabledProviders = configuration
        .GetSection(ConstsIngestion.EnabledProvidersKey)
        .Get<List<string>>() ?? [];

        foreach (var assemblyName in enabledProviders)
        {
            var assembly = Assembly.Load(assemblyName);

            var providerTypes = assembly.GetTypes()
                .Where(t => typeof(IIngestionProvider).IsAssignableFrom(t)
                         && !t.IsInterface
                         && !t.IsAbstract);

            foreach (var type in providerTypes)
                services.AddScoped(typeof(IIngestionProvider), type);
        }

        services.AddScoped<IIngestionService, IngestionService>();
        return services;
    }
}
