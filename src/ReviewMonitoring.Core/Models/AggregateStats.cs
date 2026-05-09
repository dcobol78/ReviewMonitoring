namespace ReviewMonitoring.Core.Models;

public class AggregateStats
{
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public Dictionary<int, int> Distribution { get; set; } = []; // Солько каких оценок
    public required AnalysisSummary Analysis { get; set; }
}
