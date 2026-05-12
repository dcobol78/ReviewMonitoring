using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Interfaces;

public interface IProcessingService
{
    Task<ProcessingResult> ProcessSingleAsync(ProcessingRequest request, CancellationToken ct);
    Task<ProcessingResult> ProcessFinalAsync(ProcessingRequest request, CancellationToken ct);
}
