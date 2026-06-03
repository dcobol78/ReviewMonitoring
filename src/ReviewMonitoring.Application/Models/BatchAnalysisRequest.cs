using ReviewMonitoring.Domain.Models;
namespace ReviewMonitoring.Application.Models;
public class BatchAnalysisRequest
{
    public required IReadOnlyCollection<ListingReviews> ListingReviews { get; set; }
    public required IReadOnlyCollection<SourceResult> Sources { get; set; }
}
