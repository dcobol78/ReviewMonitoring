using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;

public class ProcessingFinalRequest
{
    public required IReadOnlyCollection<ListingReviews> ListingReviews { get; set; }
    public required IReadOnlyCollection<SourceResult> Sources { get; set; }
    public ProcessingMode Mode { get; set; }
}
