using Microsoft.EntityFrameworkCore;
using ReviewMonitoring.Infrastructure.Postgres.Configuration;
using ReviewMonitoring.Infrastructure.Postgres.Entities;

namespace ReviewMonitoring.Infrastructure.Postgres;

public class PostgresDbContext : DbContext
{
    public DbSet<JobEntity> Jobs { get; set; }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new JobEntityConfiguration(Database.IsNpgsql()));
    }
}