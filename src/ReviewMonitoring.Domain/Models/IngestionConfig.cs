using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.Domain.Models;
public class IngestionConfig
{
    public int MaxCandidatesPerProvider { get; set; } = 10;
    public int MaxFinalMatches { get; set; } = 5;
    public MatchStrictness Strictness { get; set; } = MatchStrictness.Medium;
    public int MaxReviewsPerListing { get; set; } = 300;
    public bool ParseAdditionalSellers { get; set; }
}