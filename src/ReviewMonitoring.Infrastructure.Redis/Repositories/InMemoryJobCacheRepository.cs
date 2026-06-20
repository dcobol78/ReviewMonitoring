using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Domain;
using System.Collections.Concurrent;

namespace ReviewMonitoring.Infrastructure.Redis.Repositories;
public class InMemoryJobCacheRepository : IJobCacheRepository
{
    private readonly ConcurrentDictionary<Guid, Job> _store = new();

    public Task SetAsync(Job job)
    {
        _store[job.Id] = job;
        return Task.CompletedTask;
    }

    public Task<Job?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var job);
        return Task.FromResult(job);
    }

    public Task DeleteAsync(Guid id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task PingAsync(Guid id)
    {
        return Task.CompletedTask;
    }
}