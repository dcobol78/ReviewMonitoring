using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.AI.Services;
internal class MockReviewAnalyzer : IReviewAnalyzer
{
    public Task<AnalysisSummary> AnalyzeAsync(IReadOnlyList<Review> reviews, CancellationToken ct)
    {
        return Task.FromResult(new AnalysisSummary
        {
            Pros = ["Mock pro 1", "Mock pro 2"],
            Cons = ["Mock con 1"],
            Keywords = ["mock", "keyword"],
            Flags = [],
            Summary = $"Mock analysis of {reviews.Count} reviews"
        });
    }

    public Task<BatchAnalysisResult> AnalyzeBatchAsync(BatchAnalysisRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
