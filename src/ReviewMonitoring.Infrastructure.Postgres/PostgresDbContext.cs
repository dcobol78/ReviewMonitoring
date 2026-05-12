using Microsoft.EntityFrameworkCore;
using ReviewMonitoring.Infrastructure.Postgres.Entities;

namespace ReviewMonitoring.Infrastructure.Postgres;

public class PostgresDbContext : DbContext
{
    public DbSet<JobEntity> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
    }
}