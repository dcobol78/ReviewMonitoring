using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewMonitoring.Infrastructure.Postgres.Entities;

namespace ReviewMonitoring.Infrastructure.Postgres.Configuration;
public class JobEntityConfiguration : IEntityTypeConfiguration<JobEntity>
{
    private readonly bool _isNpgsql;
    public JobEntityConfiguration(bool isNpgsql) => _isNpgsql = isNpgsql;
    
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        var jsonType = _isNpgsql ? "jsonb" : "TEXT";

        builder.Property(x => x.Result)
            .HasColumnType("jsonb");

        builder.Property(x => x.SearchConfig)
            .HasColumnType("jsonb");

        builder.Property(x => x.SourceStatuses)
            .HasColumnType("jsonb");
    }
}