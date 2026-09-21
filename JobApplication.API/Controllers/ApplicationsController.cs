using JobApplication.Application.DTOs;
using JobApplication.Application.Features.JobCandidateApplication.Commands.ApplyApplication;
using JobApplication.Application.Features.JobCandidateApplication.Commands.CancelApplication;
using JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationById;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetCandidateApplications;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetJobApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {
            var result = await _mediator.Send(new GetCandidateApplicationsQuery());
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetJobApplications(int jobId)
        {
            var result = await _mediator.Send(new GetJobApplicationsQuery(jobId));
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetApplicationByIdQuery(id));
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Apply([FromBody] ApplyJobRequest request)
        {
            var result = await _mediator.Send(new ApplyApplicationCommand { JobId = request.JobId });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateApplicationStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateApplicationStatusCommand { ApplicationId = id, Status = dto.Status });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _mediator.Send(new CancelApplicationCommand { ApplicationId = id });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return NoContent();
        }
    }
}
