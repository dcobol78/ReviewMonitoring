using ReviewMonitoring.Domain.Domain;

namespace ReviewMonitoring.Application.Interfaces;
public interface IJobRepository
{
    Task SaveAsync(Job entity);
    Task<Job?> GetByIdAsync(Guid id);
}