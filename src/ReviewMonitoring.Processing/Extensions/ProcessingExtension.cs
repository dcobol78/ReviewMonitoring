using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;

namespace ReviewMonitoring.Processing.Extensions;
public static class ProcessingExtension
{
    public static IServiceCollection AddProcessing(
        this IServiceCollection services)
    {
        services.AddScoped<IProcessingService, ProcessingService>();
        return services;
    }
}
