using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Infrastructure.Postgres.Entities;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ReviewMonitoring.Infrastructure.Postgres.Repoistories;
public class JobRepository : IJobRepository
{
    private readonly PostgresDbContext _db;

    public JobRepository(PostgresDbContext db)
    {
        _db = db;
    }

    public async Task SaveAsync(Job job)
    {
        var entity = ToEntity(job);
        await _db.Jobs.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Jobs.FindAsync(id);
        return entity is null ? null : ToDomain(entity);
    }

    private static JobEntity ToEntity(Job job)
    {
        string? result = null;
        if (job.Result is not null)
            result = JsonSerializer.Serialize(job.Result);

        return new JobEntity
        {
            Id = job.Id,
            Query = job.Query,
            Mode = job.Mode.ToString(),
            Status = job.Status,
            CreatedAt = job.CreatedAt,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage,
            Result = result
        };
    }

    private static Job ToDomain(JobEntity entity)
    {
        ProcessingResult? result = null;
        if (entity.Result is not null)
            result = JsonSerializer.Deserialize<ProcessingResult>(entity.Result);

        return new Job
        {
            Id = entity.Id,
            Query = entity.Query,
            Mode = Enum.Parse<ProcessingMode>(entity.Mode),
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            ErrorMessage = entity.ErrorMessage,
            Result = result
        };
    }

}
