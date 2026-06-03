using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReviewMonitoring.Infrastructure.Postgres.Consts;


namespace ReviewMonitoring.Infrastructure.Postgres;
public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseNpgsql(ConstsPostgresDebug.DebugPostgresConnection)
            .Options;

        return new PostgresDbContext(options);
    }
}
