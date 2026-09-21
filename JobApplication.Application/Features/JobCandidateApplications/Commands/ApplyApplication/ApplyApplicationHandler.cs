using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplication.Commands.ApplyApplication
{
    public class ApplyApplicationHandler : IRequestHandler<ApplyApplicationCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ApplyApplicationHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<Result<int>> Handle(ApplyApplicationCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result<int>.Failure(new Error(404, "Job not found."));

            if (!job.IsActive)
                return Result<int>.Failure(new Error(400, "Cannot apply to a closed job."));

            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (candidate == null)
                return Result<int>.Failure(new Error(403, "Candidate profile not found."));

            var alreadyApplied = await _unitOfWork.Applications.ExistsAsync(candidate.Id, request.JobId, cancellationToken);
            if (alreadyApplied)
                return Result<int>.Failure(new Error(400, "You have already applied to this job."));

            var application = new Domain.Entities.JobCandidateApplication
            {
                CandidateId = candidate.Id,
                JobId = request.JobId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Applications.InsertAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(application.Id);
        }
    }
}
