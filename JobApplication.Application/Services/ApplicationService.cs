using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<int> ApplyAsync(ApplyJobRequest request, CancellationToken cancellationToken = default)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                throw new KeyNotFoundException("Job not found.");

            if (!job.IsActive)
                throw new InvalidOperationException("Cannot apply to a closed job.");

            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (candidate == null)
                throw new UnauthorizedAccessException("Candidate profile not found.");

            var alreadyApplied = await _unitOfWork.Applications.ExistsAsync(candidate.Id, request.JobId, cancellationToken);
            if (alreadyApplied)
                throw new InvalidOperationException("You have already applied to this job.");

            var application = new JobCandidateApplication
            {
                CandidateId = candidate.Id,
                JobId = request.JobId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                StatusUpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Applications.InsertAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return application.Id;
        }

        public async Task CancelAsync(int applicationId, CancellationToken cancellationToken = default)
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(applicationId, cancellationToken);
            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (candidate == null || application.CandidateId != candidate.Id)
                throw new UnauthorizedAccessException("You do not own this application.");

            application.Cancel();

            _unitOfWork.Applications.Update(application);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
