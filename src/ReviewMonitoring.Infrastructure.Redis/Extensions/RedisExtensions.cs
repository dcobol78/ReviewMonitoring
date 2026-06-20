using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Infrastructure.Redis.Consts;
using ReviewMonitoring.Infrastructure.Redis.Repositories;
using ReviewMonitoring.Shared.Consts;
using ReviewMonitoring.Shared.Extensions;
using StackExchange.Redis;

namespace ReviewMonitoring.Infrastructure.Redis.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration.IsDemoMode())
        {
            services.AddSingleton<IJobCacheRepository, InMemoryJobCacheRepository>();
            return services;
        }

        string? redisConnection = configuration.GetConnectionString(ConstsRedis.ConnectionStringKey);
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
#if DEBUG
            redisConnection = ConstsRedisDebug.DebugRedisConnection;
#else
            throw new KeyNotFoundException("Не указан RedisConnectionString");
#endif
        }

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddScoped<IJobCacheRepository, JobCacheRepository>();

        return services;
    }
}