using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Infrastructure.Postgres.Consts;
using ReviewMonitoring.Infrastructure.Postgres.Repoistories;
using ReviewMonitoring.Shared.Consts;
using ReviewMonitoring.Shared.Extensions;
using System;

namespace ReviewMonitoring.Infrastructure.Postgres.Extensions;
public static class PostgresExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration.IsDemoMode())
        {
            var keepAlive = new SqliteConnection("DataSource=DemoDb;Mode=Memory;Cache=Shared");
            keepAlive.Open();
            services.AddSingleton(keepAlive);

            services.AddDbContext<PostgresDbContext>((sp, opt) =>
                opt.UseSqlite(sp.GetRequiredService<SqliteConnection>()));
        }
        else
        {
            string? postgresConnection = configuration.GetConnectionString(ConstsPostgres.ConnectionStringKey);
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
        }

        services.AddScoped<IJobRepository, JobRepository>();
        return services;
    }
}
