using MediatR;
using ReviewMonitoring.Application.Interfaces;

namespace ReviewMonitoring.Application.Commands;

public class PingJobCommandHandler : IRequestHandler<PingJobCommand, bool>
{
    private readonly IJobCacheRepository _cache;

    public PingJobCommandHandler(IJobCacheRepository cache)
    {
        _cache = cache;
    }

    public async Task<bool> Handle(PingJobCommand request, CancellationToken ct)
    {
        var job = await _cache.GetByIdAsync(request.jobId);
        if (job is null)
            return false;

        await _cache.PingAsync(request.jobId);
        return true;
    }
}