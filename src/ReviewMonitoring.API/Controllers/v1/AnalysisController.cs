using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReviewMonitoring.Application.Commands;
using ReviewMonitoring.Application.Models;

namespace ReviewMonitoring.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalysisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/v1/analysis
        [HttpPost]
        public async Task<IActionResult> Analyze(
            [FromBody] AnalysisRequest request,
            CancellationToken ct)
        {
            var command = new CreateJobCommand(
                request.Query,
                request.Mode);

            var jobId = await _mediator.Send(command, ct);

            return Accepted(new { jobId });
        }

        // GET /api/v1/analysis/search?q=Elden+Ring
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            throw new NotImplementedException();
        }
    }
}
