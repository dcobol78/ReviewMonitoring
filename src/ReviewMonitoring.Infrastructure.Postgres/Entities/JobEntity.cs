using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.Infrastructure.Postgres.Entities;
public class JobEntity
{
    public Guid Id { get; set; }
    public required string Query { get; set; }
    public required string Mode { get; set; }
    public JobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Result { get; set; }
}