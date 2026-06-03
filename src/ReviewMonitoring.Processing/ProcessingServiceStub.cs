using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Processing;
public class ProcessingServiceStub : IProcessingService
{
    public Task<SourceResult> ProcessSingleAsync(
        ProcessingSingleRequest request,
        CancellationToken ct)
    {
        return Task.FromResult(new SourceResult
        {
            Name = request.ProviderName,
            Url = string.Empty
        });
    }

    public Task<ProcessingResult> ProcessFinalAsync(
        ProcessingFinalRequest request,
        CancellationToken ct)
    {
        return Task.FromResult(new ProcessingResult());
    }
}
