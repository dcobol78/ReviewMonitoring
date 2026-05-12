namespace ReviewMonitoring.Domain.Models;

public class ProcessingResult
{
    public required ProductInfo Product { get; set; }
    public required AggregateStats Aggregate { get; set; }
    public List<SourceResult> Sources { get; set; } = [];
}

