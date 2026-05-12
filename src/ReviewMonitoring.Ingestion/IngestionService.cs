using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Ingestion.Interfaces;

namespace ReviewMonitoring.Ingestion;
public class IngestionService : IIngestionService
{
    private readonly IEnumerable<IIngestionProvider> _providers;
    
    public IngestionService(IEnumerable<IIngestionProvider> providers)
    {
        _providers = providers;
    }

    public IReadOnlyList<string> GetEnabledProviders()
    {
        return _providers.Select(p => p.Name).ToList();
    }

    public Task<IngestionResult> IngestAsync(IngestionRequest request, IProgress<IngestionProgress>? progress, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
