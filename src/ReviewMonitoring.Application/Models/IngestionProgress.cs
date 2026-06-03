using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;
public class IngestionProgress
{
    public required string ProviderName { get; set; }
    
    public required string OriginalUrl { get; set; }

    public SourceStatus Status { get; set; }

    public required IReadOnlyList<ListingReviews> Listings { get; set; }
    
    public IReadOnlyList<Review> GetReviews()
    {
        return Listings.SelectMany(l => l.Reviews).ToList();
    }
    
    public int ReviewsCollected => GetReviews().Count;
    
    public int ListingsCollected => Listings.Count;

}
