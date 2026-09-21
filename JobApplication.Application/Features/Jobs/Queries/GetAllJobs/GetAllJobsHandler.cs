using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsHandler : IRequestHandler<GetAllJobsQuery, Result<IReadOnlyList<JobDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllJobsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<JobDto>>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _unitOfWork.Jobs.GetActiveJobsWithRecruiterAsync(request.SearchTerm, cancellationToken);

            var jobDtos = jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                IsActive = j.IsActive,
                RecruiterId = j.RecruiterId,
                CompanyName = j.Recruiter?.CompanyName ?? string.Empty
            }).ToList();

            return Result<IReadOnlyList<JobDto>>.Success(jobDtos);
        }
    }
}
