using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Services;

namespace ReviewMonitoring.Application.Extensions;
public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ApplicationExtensions).Assembly);
        });

        services.AddSingleton<JobProcessor>();

        return services;
    }
}
