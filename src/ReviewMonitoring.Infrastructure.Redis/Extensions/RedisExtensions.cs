using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Infrastructure.Postgres.Consts;
using ReviewMonitoring.Infrastructure.Redis.Repositories;
using StackExchange.Redis;

namespace ReviewMonitoring.Infrastructure.Redis.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? redisConnection = configuration.GetConnectionString("Redis");
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