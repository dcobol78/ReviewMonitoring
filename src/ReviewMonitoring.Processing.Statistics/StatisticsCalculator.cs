// Processing/StatisticsCalculator.cs
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Processing;

internal class StatisticsCalculator : IStatisticsCalculator
{
    public ListingResult BuildListingResult(Listing listing)
    {
        var distribution = BuildDistribution(listing.Reviews);

        return new ListingResult
        {
            Url = listing.Url,
            SellerName = listing.SellerName,
            ProductTitle = listing.ProductTitle,
            Price = listing.Price,
            Rating = listing.Rating,
            ReviewCount = listing.ReviewCount,
            Distribution = distribution,
            ImageUrls = listing.ImageUrls,
            Extras = listing.Extras
        };
    }

    public (decimal AverageRating, int TotalReviews, Dictionary<int, int> Distribution)
        CalculateForListings(IReadOnlyCollection<Listing> listings)
    {
        var allReviews = listings.SelectMany(l => l.Reviews).ToList();
        var distribution = BuildDistribution(allReviews);
        var total = allReviews.Count;

        var average = total > 0
            ? Math.Round((decimal)allReviews.Average(r => r.Rating), 2)
            : 0;

        return (average, total, distribution);
    }

    public AggregateStats CalculateAggregate(IReadOnlyCollection<SourceResult> sources)
    {
        var allSellers = sources.SelectMany(s => s.Sellers).ToList();

        // Объединяем распределения всех источников
        var distribution = new Dictionary<int, int>();
        foreach (var source in sources)
            foreach (var (rating, count) in source.RatingDistribution)
                distribution[rating] = distribution.GetValueOrDefault(rating) + count;

        var totalReviews = sources.Sum(s => s.TotalReviews);

        // Средневзвешенная оценка по распределению
        var weightedSum = distribution.Sum(kv => kv.Key * kv.Value);
        var totalRated = distribution.Values.Sum();
        var averageRating = totalRated > 0
            ? Math.Round((decimal)weightedSum / totalRated, 2)
            : 0;

        var prices = allSellers
            .Where(s => s.Price > 0)
            .Select(s => s.Price)
            .ToList();

        return new AggregateStats
        {
            TotalSources = sources.Count,
            TotalSellers = allSellers.Count,
            TotalReviews = totalReviews,
            AverageRating = averageRating,
            MinPrice = prices.Count > 0 ? prices.Min() : 0,
            MaxPrice = prices.Count > 0 ? prices.Max() : 0,
            Distribution = distribution,
            BestSeller = SelectBestSeller(allSellers)
        };
    }

    private static Dictionary<int, int> BuildDistribution(IEnumerable<Review> reviews)
    {
        var distribution = new Dictionary<int, int>();
        foreach (var review in reviews)
        {
            if (review.Rating is >= 1 and <= 5)
                distribution[review.Rating] = distribution.GetValueOrDefault(review.Rating) + 1;
        }
        return distribution;
    }

    private static ListingResult? SelectBestSeller(List<ListingResult> sellers)
    {
        // Лучший = высокий рейтинг + достаточно отзывов + адекватная цена
        return sellers
            .Where(s => s.ReviewCount > 0)
            .OrderByDescending(s => s.Rating)
            .ThenByDescending(s => s.ReviewCount)
            .FirstOrDefault();
    }
}