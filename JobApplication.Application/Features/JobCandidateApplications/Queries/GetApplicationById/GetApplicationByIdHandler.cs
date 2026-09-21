using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationById
{
    public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, Result<JobApplicationDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetApplicationByIdHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<JobApplicationDetailsDto>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.Applications.GetWithDetailsByIdAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result<JobApplicationDetailsDto>.Failure(new Error(404, "Application not found."));

            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);

            bool isOwnerCandidate = candidate != null && application.CandidateId == candidate.Id;
            bool isJobOwnerRecruiter = recruiter != null && application.Job != null && application.Job.RecruiterId == recruiter.Id;

            if (!isOwnerCandidate && !isJobOwnerRecruiter)
                return Result<JobApplicationDetailsDto>.Failure(new Error(403, "You are not authorized to view this application."));

            var dto = new JobApplicationDetailsDto
            {
                Id = application.Id,
                JobId = application.JobId,
                JobTitle = application.Job?.Title ?? string.Empty,
                CandidateId = application.CandidateId,
                CandidateName = application.Candidate?.Name ?? string.Empty,
                CvUrl = application.Candidate?.CvUrl ?? string.Empty,
                Status = application.JobApplicationStatus.ToString(),
                AppliedAt = application.AppliedAt,
                UpdatedAt = application.UpdatedAt
            };

            return Result<JobApplicationDetailsDto>.Success(dto);
        }
    }
}
