using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Queries.GetJobById
{
    public class GetJobByIdHandler : IRequestHandler<GetJobByIdQuery, Result<JobDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetJobByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<JobDetailsDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetWithRecruiterByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result<JobDetailsDto>.Failure(new Error(404, "Job not found."));

            var dto = new JobDetailsDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                IsActive = job.IsActive,
                RecruiterId = job.RecruiterId,
                CompanyName = job.Recruiter?.CompanyName ?? string.Empty
            };

            return Result<JobDetailsDto>.Success(dto);
        }
    }
}
