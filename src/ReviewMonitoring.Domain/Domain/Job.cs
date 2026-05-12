using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Domain.Domain;

public class Job
{
    public required Guid Id { get; set; }
    public required string Query { get; set; }
    public required ProcessingMode Mode { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public Dictionary<string, SourceStatus> SourceStatuses { get; set; } = [];
    public int ReviewsCollected { get; set; }
    public ProcessingResult? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastPingAt { get; set; }
}
