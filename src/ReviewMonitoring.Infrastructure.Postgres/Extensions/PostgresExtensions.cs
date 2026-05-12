using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Infrastructure.Postgres.Consts;
using ReviewMonitoring.Infrastructure.Postgres.Repoistories;

namespace ReviewMonitoring.Infrastructure.Postgres.Extensions;
public static class PostgresExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? postgresConnection = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(postgresConnection))
        {

#if DEBUG
            postgresConnection = ConstsPostgresDebug.DebugPostgresConnection;
#else
            throw new KeyNotFoundException("Не указан PostgresConnectionString");
#endif

        }

        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(postgresConnection));

        services.AddScoped<IJobRepository, JobRepository>();

        return services;
    }
}
