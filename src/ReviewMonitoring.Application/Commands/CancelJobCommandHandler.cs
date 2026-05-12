using MediatR;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.Application.Commands;

public class CancelJobCommandHandler : IRequestHandler<CancelJobCommand, bool>
{
    private readonly IJobCacheRepository _cache;
    private readonly IJobRepository _repository;

    public CancelJobCommandHandler(IJobCacheRepository cache, IJobRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<bool> Handle(CancelJobCommand request, CancellationToken ct)
    {
        var job = await _cache.GetByIdAsync(request.JobId);

        if (job is null)
            return false;

        if (job.Status is JobStatus.Completed
            or JobStatus.PartiallyCompleted
            or JobStatus.Failed
            or JobStatus.Cancelled)
            return false;

        job.Status = JobStatus.Cancelled;
        job.CompletedAt = DateTime.UtcNow;

        await _repository.SaveAsync(job);
        await _cache.DeleteAsync(request.JobId);

        return true;
    }
}
