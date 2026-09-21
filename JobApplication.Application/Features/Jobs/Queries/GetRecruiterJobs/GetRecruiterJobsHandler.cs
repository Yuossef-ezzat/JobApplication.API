using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Queries.GetRecruiterJobs
{
    public class GetRecruiterJobsHandler : IRequestHandler<GetRecruiterJobsQuery, Result<IReadOnlyList<JobDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetRecruiterJobsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<IReadOnlyList<JobDto>>> Handle(GetRecruiterJobsQuery request, CancellationToken cancellationToken)
        {
            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null)
                return Result<IReadOnlyList<JobDto>>.Failure(new Error(403, "Recruiter profile not found."));

            var jobs = await _unitOfWork.Jobs.GetByRecruiterIdAsync(recruiter.Id, cancellationToken);

            var dtos = jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                IsActive = j.IsActive,
                RecruiterId = j.RecruiterId,
                CompanyName = recruiter.CompanyName
            }).ToList();

            return Result<IReadOnlyList<JobDto>>.Success(dtos);
        }
    }
}
