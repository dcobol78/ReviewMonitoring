using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Infrastructure.Postgres.Entities;
using System.Text.Json;

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

        string config = JsonSerializer.Serialize(job.SearchConfig);

        string sourceStatuses =
        JsonSerializer.Serialize(job.SourceStatuses);

        return new JobEntity
        {
            Id = job.Id,
            Query = job.Query,
            Mode = job.Mode.ToString(),
            Status = job.Status,
            CreatedAt = job.CreatedAt,
            SearchConfig = config,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage,
            Result = result,
            SourceStatuses = sourceStatuses
        };
    }

    private static Job ToDomain(JobEntity entity)
    {
        ProcessingResult? result = null;
        if (entity.Result is not null)
            result = JsonSerializer.Deserialize<ProcessingResult>(entity.Result);

        var searchConfig = JsonSerializer.Deserialize<IngestionConfig>(entity.SearchConfig) ?? new IngestionConfig();

        var sourceStatuses =
        JsonSerializer.Deserialize<Dictionary<string, SourceStatus>>(
            entity.SourceStatuses)
        ?? [];

        return Job.Restore(
            id: entity.Id,
            query: entity.Query,
            mode: Enum.Parse<ProcessingMode>(entity.Mode),
            status: entity.Status,
            createdAt: entity.CreatedAt,
            searchConfig: searchConfig,
            completedAt: entity.CompletedAt,
            errorMessage: entity.ErrorMessage,
            result: result,
            sourceStatuses: sourceStatuses);
    }
}
