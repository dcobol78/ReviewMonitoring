using ReviewMonitoring.Domain.Domain;

namespace ReviewMonitoring.Application.Interfaces;

public interface IJobCacheRepository
{
    Task SetAsync(Job job);
    Task<Job?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task PingAsync(Guid id);
}