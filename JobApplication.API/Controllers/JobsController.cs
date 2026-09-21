using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Job.Command.CreateJob;
using JobApplication.Application.Features.Jobs.Command.CloseJob;
using JobApplication.Application.Features.Jobs.Command.DeleteJob;
using JobApplication.Application.Features.Jobs.Command.UpdateJob;
using JobApplication.Application.Features.Jobs.Queries.GetAllJobs;
using JobApplication.Application.Features.Jobs.Queries.GetJobById;
using JobApplication.Application.Features.Jobs.Queries.GetRecruiterJobs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetAllJobsQuery { SearchTerm = search });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetJobByIdQuery(id));
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpGet("my-jobs")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetMyJobs()
        {
            var result = await _mediator.Send(new GetRecruiterJobsQuery());
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Create([FromBody] CreateJobDto createJobDto)
        {
            var result = await _mediator.Send(new CreateJobCommand { Title = createJobDto.Title, Description = createJobDto.Description });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobDto dto)
        {
            var result = await _mediator.Send(new UpdateJobCommand { JobId = id, Title = dto.Title, Description = dto.Description });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return NoContent();
        }

        [HttpPut("{id}/close")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Close(int id)
        {
            var result = await _mediator.Send(new CloseJobCommand { JobId = id });
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteJobCommand(id));
            if (result.IsFailure)
                return StatusCode(result.Error.Code, new { error = result.Error.Message });

            return NoContent();
        }
    }
}
