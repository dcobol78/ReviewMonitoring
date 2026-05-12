using MediatR;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Commands;
public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Guid>
{
    private readonly IJobCacheRepository _cache;
    private readonly IIngestionService _ingestion;
    private readonly IJobRepository _repository;
    private readonly IProcessingService _processing;


    public CreateJobCommandHandler(IJobCacheRepository cache, 
        IIngestionService ingestion, 
        IJobRepository repository, 
        IProcessingService processing)
    {
        _cache = cache;
        _ingestion = ingestion;
        _repository = repository;
        _processing = processing;
    }

    public async Task<Guid> Handle(CreateJobCommand request, CancellationToken ct)
    {
        var availableProviders = _ingestion.GetEnabledProviders();

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Query = request.query,
            Mode = request.mode,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SourceStatuses = availableProviders.ToDictionary(p => p, p => SourceStatus.Pending)
        };
        
        await _cache.SetAsync(job);

        _ = ProcessJobAsync(job, CancellationToken.None);

        return job.Id;
    }

    //Возможно стоит сделать отдельным воркером
    private async Task ProcessJobAsync(Job job, CancellationToken ct)
    {
        job.Status = JobStatus.Parsing;
        await _cache.SetAsync(job);

        var allReviews = new List<Review>();

        var progress = new Progress<IngestionProgress>(async p =>
        {
            // обновляем статус источника в Job
            job.SourceStatuses[p.SourceName] = p.Status;
            job.ReviewsCollected += p.ReviewsCollected;
            allReviews.AddRange(p.Reviews);

            // клиент видит обновление через стриминг
            await _cache.SetAsync(job);
        });

        await _ingestion.IngestAsync(
            new IngestionRequest { Query = job.Query },
            progress,
            ct);

        job.Status = JobStatus.Analyzing;
        await _cache.SetAsync(job);

        job.Result = await _processing.ProcessAsync(
            new ProcessingRequest { Reviews = allReviews, Mode = job.Mode },
            ct);

        job.Status = JobStatus.Completed;
        job.CompletedAt = DateTime.UtcNow;

        await _repository.SaveAsync(job);
        await _cache.DeleteAsync(job.Id);
    }
}