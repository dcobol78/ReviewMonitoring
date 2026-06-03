using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;

public class ProcessingSingleRequest
{
    public required IReadOnlyCollection<ListingReviews> Listings { get; set; }
    public required string ProviderName { get; set; }
    public required string OriginalUrl { get; set; }
}
