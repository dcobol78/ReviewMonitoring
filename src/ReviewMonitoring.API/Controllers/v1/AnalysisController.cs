using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using ReviewMonitoring.API.Models.v1;
using ReviewMonitoring.Application.Commands;

namespace ReviewMonitoring.API.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]_v1")]
    public class AnalysisController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalysisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/analysis
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

        // GET /api/analysis/search?q=Elden+Ring
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            throw new NotImplementedException();
        }
    }
}
