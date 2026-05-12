using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Ingestion.Interfaces;
using System.Reflection;

namespace ReviewMonitoring.Ingestion.Extensions;

public static class IngestionExtensions
{
    public static IServiceCollection AddIngestion(
    this IServiceCollection services,
    IEnumerable<string> enabledAssemblies)
    {
        foreach (var assemblyName in enabledAssemblies)
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
