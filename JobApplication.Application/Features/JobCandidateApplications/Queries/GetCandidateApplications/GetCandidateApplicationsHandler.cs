using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetCandidateApplications
{
    public class GetCandidateApplicationsHandler : IRequestHandler<GetCandidateApplicationsQuery, Result<List<CandidateApplicationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetCandidateApplicationsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<CandidateApplicationDto>>> Handle(GetCandidateApplicationsQuery request, CancellationToken cancellationToken)
        {
            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (candidate == null)
                return Result<List<CandidateApplicationDto>>.Failure(new Error(403, "Candidate profile not found."));

            var applications = await _unitOfWork.Applications.GetByCandidateIdWithDetailsAsync(candidate.Id, cancellationToken);

            var dtos = applications.Select(a => new CandidateApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = a.Job?.Title ?? string.Empty,
                CompanyName = a.Job?.Recruiter?.CompanyName ?? string.Empty,
                Status = a.JobApplicationStatus.ToString(),
                AppliedAt = a.AppliedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            return Result<List<CandidateApplicationDto>>.Success(dtos);
        }
    }
}
