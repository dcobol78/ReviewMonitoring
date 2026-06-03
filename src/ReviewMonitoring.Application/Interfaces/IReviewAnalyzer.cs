using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Interfaces;

//Todo: Переделать дичь, сделать абстрактнее и лучше тогда промпт в каждом свой и передавать сюда???
public interface IReviewAnalyzer
{
    Task<AnalysisSummary> AnalyzeAsync(
        IReadOnlyList<Review> reviews,
        CancellationToken ct);

    Task<BatchAnalysisResult> AnalyzeBatchAsync(
    BatchAnalysisRequest request,
    CancellationToken ct);
}