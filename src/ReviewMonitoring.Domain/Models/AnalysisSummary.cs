namespace ReviewMonitoring.Domain.Models;

public class AnalysisSummary
{
    public List<string> Pros { get; set; } = [];
    public List<string> Cons { get; set; } = [];
    public required string Summary { get; set; }
}

