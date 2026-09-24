using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using JobApplication.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JobApplication.Application.Features.JobCandidateApplication.Commands.CancelApplication
{
    public class CancelApplicationHandler : IRequestHandler<CancelApplicationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJob _backgroundJob;
        private readonly ICurrentUserService _currentUserService;
        public CancelApplicationHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBackgroundJob backgroundJob)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _backgroundJob = backgroundJob;
            
        }
        public async Task<Result> Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(request.ApplicationId, cancellationToken);
            _backgroundJob.EnqueueJob<INotificationService>(s => s.SendNotification(application.Id));
            if (application == null)
                return Result.Failure(new Error(404, "Application not found."));

            var candidate = await _unitOfWork.Candidates.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (candidate == null || application.CandidateId != candidate.Id)
                return Result.Failure(new Error(403, "You do not own this application."));

            if (application.JobApplicationStatus is JobApplicationStatus.Accepted or JobApplicationStatus.Rejected or JobApplicationStatus.Cancelled)
                return Result.Failure(new Error(400, "Application cannot be cancelled in its current state."));

            try
            {
                application.Cancel();
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
