using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Interfaces;

namespace ReviewMonitoring.Ingestion.Wildberries;

public class WildberriesIngestionProvider : IIngestionProvider
{
    public string Name => throw new NotImplementedException();

    public bool CanHandle(IngestionRequest request)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Review> IngestAsync(IngestionRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
