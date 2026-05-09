using ReviewMonitoring.Core.Enums;
using ReviewMonitoring.Core.Models;

namespace ReviewMonitoring.Core.Domain;

public class Job
{
    public required Guid Id { get; set; }
    public required string Query { get; set; }
    public required AnalysisMode Mode { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public Dictionary<string, SourceJobStatus> SourceStatuses { get; set; } = [];
    public AnalysisResult? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastPingAt { get; set; }
}
