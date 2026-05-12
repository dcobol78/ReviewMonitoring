using MediatR;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Domain;

namespace ReviewMonitoring.Application.Queries;

public class GetJobQueryHandler : IRequestHandler<GetJobQuery, Job?>
{
    private readonly IJobCacheRepository _cache;
    private readonly IJobRepository _repository;

    public GetJobQueryHandler(IJobCacheRepository cache, IJobRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<Job?> Handle(GetJobQuery request, CancellationToken ct)
    {
        // Сначала ищем в Redis — активные Job'ы
        var job = await _cache.GetByIdAsync(request.JobId);
        if (job is not null)
            return job;

        // Если нет — ищем в Postgres — завершённые Job'ы
        return await _repository.GetByIdAsync(request.JobId);
    }
}