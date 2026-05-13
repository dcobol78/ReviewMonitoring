using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Interfaces;

public interface IProcessingService
{
    Task<SourceResult> ProcessSingleAsync(ProcessingSingleRequest request, CancellationToken ct);
    Task<ProcessingResult> ProcessFinalAsync(ProcessingFinalRequest request, CancellationToken ct);
}
