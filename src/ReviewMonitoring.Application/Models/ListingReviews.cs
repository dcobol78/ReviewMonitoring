using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;
public class ListingReviews
{
    public required Listing ListingData { get; set; }
    public required IReadOnlyList<Review> Reviews { get; set; }
}