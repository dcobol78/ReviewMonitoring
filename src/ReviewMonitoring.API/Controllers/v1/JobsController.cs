using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReviewMonitoring.Application.Commands;
using ReviewMonitoring.Application.Queries;
using ReviewMonitoring.Domain.Enums;
using System.Text.Json;

namespace ReviewMonitoring.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/v1/jobs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(Guid id, CancellationToken ct)
        {
            var job = await _mediator.Send(new GetJobQuery(id), ct);

            if (job is null)
                return NotFound();

            return Ok(job);
        }

        // GET /api/v1/jobs/{id}/stream
        [HttpGet("{id}/stream")]
        public async Task StreamStatus(Guid id, CancellationToken ct)
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";

            while (!ct.IsCancellationRequested)
            {
                var job = await _mediator.Send(new GetJobQuery(id), ct);

                if (job is null)
                    break;

                var json = JsonSerializer.Serialize(job);
                await Response.WriteAsync($"data: {json}\n\n", ct);
                await Response.Body.FlushAsync(ct);

                if (job.Status is JobStatus.Completed
                    or JobStatus.PartiallyCompleted
                    or JobStatus.Failed
                    or JobStatus.Cancelled)
                    break;

                await Task.Delay(1000, ct);
            }
        }

        // DELETE /api/v1/jobs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new CancelJobCommand(id), ct);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // POST /api/v1/jobs/{id}/ping
        [HttpPost("{id}/ping")]
        public async Task<IActionResult> Ping(Guid id, CancellationToken ct)
        {
            var job = await _mediator.Send(new GetJobQuery(id), ct);

            if (job is null)
                return NotFound();

            var result = await _mediator.Send(new PingJobCommand(id), ct);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
