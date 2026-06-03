using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Processing.Interfaces;

public interface IStatisticsCalculator
{
    (decimal AverageRating, int TotalReviews, Dictionary<int, int> Distribution) CalculateForListings(IReadOnlyCollection<ListingReviews> listings);

    AggregateStats CalculateAggregate(IReadOnlyCollection<SourceResult> sources);
}