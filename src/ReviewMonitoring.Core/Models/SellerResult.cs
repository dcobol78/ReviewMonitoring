namespace ReviewMonitoring.Core.Models;

public class SellerResult
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public List<string> Flags { get; set; } = [];
    public required AnalysisSummary Analysis { get; set; }
}
