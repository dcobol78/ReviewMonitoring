
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Domain;

namespace ReviewMonitoring.Application.Interfaces;
public interface IIngestionService
{
    Task<IngestionResult> IngestAsync(
        AnalysisRequest request,
        IProgress<IngestionProgress>? progress,
        CancellationToken ct);

    IReadOnlyList<string> GetEnabledProviders();
}
