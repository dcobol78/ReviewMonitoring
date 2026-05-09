namespace ReviewMonitoring.Core.Models;

public class AnalysisResult
{
    public required ProductInfo Product { get; set; }
    public required AggregateStats Aggregate { get; set; }
    public List<SourceResult> Sources { get; set; } = [];
}

