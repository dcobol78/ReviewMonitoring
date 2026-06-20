using Microsoft.Extensions.Configuration;
using ReviewMonitoring.Shared.Consts;

namespace ReviewMonitoring.Shared.Extensions;

//поприколу, просто чтобы вынести проверку на Demomoede
public static class ConfigurationExtensions
{
    public static bool IsDemoMode(this IConfiguration configuration)
    {
        bool.TryParse(configuration[ConstsShared.Keys.DemoMode], out var demo);
        return demo;
    }
}
