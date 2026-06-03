using MediatR;
using Microsoft.Extensions.Logging;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Application.Services;
using ReviewMonitoring.Domain.Domain;
using ReviewMonitoring.Domain.Models;
using System.Threading.Channels;

//TODO: Добавить обработку ошибок и сами ошибки тоже можжно добавить впринципе
//TODO: Logs

namespace ReviewMonitoring.Application.Commands;
public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Guid>
{
    private readonly IJobCacheRepository _cache;
    private readonly IIngestionService _ingestion;
    private readonly ILogger<CreateJobCommandHandler> _log;
    private readonly JobProcessor _processor;


    public CreateJobCommandHandler(
        IJobCacheRepository cache,
        IIngestionService ingestion,
        JobProcessor processor,
        ILogger<CreateJobCommandHandler> log)
    {
        _cache = cache;
        _ingestion = ingestion;
        _processor = processor;
        _log = log;
    }

    public async Task<Guid> Handle(CreateJobCommand request, CancellationToken ct)
    {
        var availableProviders = _ingestion.GetEnabledProviders();

        var job = Job.Create(request.query, request.mode, availableProviders);
        
        await _cache.SetAsync(job);

        _log.LogInformation("Job {JobId} created for query {Query}", job.Id, job.Query);

        _ = _processor.ProcessAsync(job, CancellationToken.None);

        _log.LogInformation("Job {JobId} processing started", job.Id);

        return job.Id;
    }
}