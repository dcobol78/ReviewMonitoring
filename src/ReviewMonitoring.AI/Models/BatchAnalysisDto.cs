
namespace ReviewMonitoring.AI.Models;

internal class BatchAnalysisDto
{
    public List<ListingAnalysisDto> Listings { get; set; } = [];
    public AnalysisDto Overall { get; set; } = new();
    public string? BestSeller { get; set; }
}
