using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Ingestion.Interfaces;
public interface IIngestionProvider
{
    string Name { get; }

    bool CanHandle(IngestionRequest request);

    IAsyncEnumerable<Review> IngestAsync(
        IngestionRequest request,
        CancellationToken ct);
}
