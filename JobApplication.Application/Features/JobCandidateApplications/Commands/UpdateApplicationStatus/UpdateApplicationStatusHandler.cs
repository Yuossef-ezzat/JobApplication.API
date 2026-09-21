using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using JobApplication.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus
{
    public class UpdateApplicationStatusHandler : IRequestHandler<UpdateApplicationStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateApplicationStatusHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.Applications.GetWithDetailsByIdAsync(request.ApplicationId, cancellationToken);
            if (application == null)
                return Result.Failure(new Error(404, "Application not found."));

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || application.Job?.RecruiterId != recruiter.Id)
                return Result.Failure(new Error(403, "You do not own the job associated with this application."));

            if (application.JobApplicationStatus == JobApplicationStatus.Cancelled)
                return Result.Failure(new Error(400, "Cannot update status of a cancelled application."));

            if (request.Status == JobApplicationStatus.Cancelled)
                return Result.Failure(new Error(400, "Recruiters cannot cancel applications."));

            try
            {
                application.UpdateStatus(request.Status);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error(400, ex.Message));
            }

            _unitOfWork.Applications.Update(application);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
