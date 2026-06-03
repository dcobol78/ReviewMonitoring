using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Ingestion.Interfaces;
using ReviewMonitoring.Ingestion.Models;

namespace ReviewMonitoring.Ingestion.AliExpress;

public class AliexpressIngestionProvider : IIngestionProvider
{
    public string ProviderName => throw new NotImplementedException();

    public bool CanHandle(string? url)
    {
        throw new NotImplementedException();
    }

    public Task<string?> FetchTitleAsync(string url, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IngestionProgress> IngestAsync(IngestionRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductCandidate>> SearchAsync(ProductSearchTerms terms, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
