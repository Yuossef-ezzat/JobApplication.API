using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetJobApplications
{
    public class GetJobApplicationsHandler : IRequestHandler<GetJobApplicationsQuery, Result<IReadOnlyList<JobApplicationDetailsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetJobApplicationsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<IReadOnlyList<JobApplicationDetailsDto>>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result<IReadOnlyList<JobApplicationDetailsDto>>.Failure(new Error(404, "Job not found."));

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || job.RecruiterId != recruiter.Id)
                return Result<IReadOnlyList<JobApplicationDetailsDto>>.Failure(new Error(403, "You do not own this job."));

            var applications = await _unitOfWork.Applications.GetByJobIdWithCandidateAsync(request.JobId, cancellationToken);

            var dtos = applications.Select(a => new JobApplicationDetailsDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = job.Title,
                CandidateId = a.CandidateId,
                CandidateName = a.Candidate?.Name ?? string.Empty,
                CvUrl = a.Candidate?.CvUrl ?? string.Empty,
                Status = a.JobApplicationStatus.ToString(),
                AppliedAt = a.AppliedAt,
                UpdatedAt = a.UpdatedAt.Value
            }).ToList();

            return Result<IReadOnlyList<JobApplicationDetailsDto>>.Success(dtos);
        }
    }
}
