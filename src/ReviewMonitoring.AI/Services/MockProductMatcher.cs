using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.AI.Services;
internal class MockProductMatcher : IProductMatcher
{
    public Task<ProductSearchTerms> ExtractSearchTermsAsync(string title, CancellationToken ct)
    {
        return Task.FromResult(new ProductSearchTerms { Exact = title });
    }

    public Task<List<ProductCandidate>> FilterMatchesAsync(string originalTitle, List<ProductCandidate> candidates, IngestionConfig config, CancellationToken ct)
    {
        if (candidates.Any())
        {
            return Task.FromResult(candidates[..1]);
        }

        return Task.FromResult(candidates);
    }
}
