using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Processing;
public class ProcessingService : IProcessingService
{
    public Task<ProcessingResult> ProcessFinalAsync(ProcessingFinalRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<SourceResult> ProcessSingleAsync(ProcessingSingleRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
