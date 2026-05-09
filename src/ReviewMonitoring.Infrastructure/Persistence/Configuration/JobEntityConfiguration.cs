using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReviewMonitoring.Core.Models;
using ReviewMonitoring.Infrastructure.Persistence.Entities;
using System.Text.Json;

namespace ReviewMonitoring.Infrastructure.Persistence.Configuration;
public class JobEntityConfiguration : IEntityTypeConfiguration<JobEntity>
{
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.Result)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<AnalysisResult>(v, JsonSerializerOptions.Default)
            );
    }
}