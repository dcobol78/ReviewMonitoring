using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Interfaces;

public interface IProductMatcher
{
    Task<ProductSearchTerms> ExtractSearchTermsAsync(
        string productTitle,
        CancellationToken ct);

    Task<List<ProductCandidate>> FilterMatchesAsync(
        string originalTitle,
        List<ProductCandidate> candidates,
        IngestionConfig config,
        CancellationToken ct);
}
