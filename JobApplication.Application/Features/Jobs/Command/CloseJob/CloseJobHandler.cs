using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Command.CloseJob
{
    public class CloseJobHandler : IRequestHandler<CloseJobCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CloseJobHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result.Failure(new Error(404, "Job not found."));

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || job.RecruiterId != recruiter.Id)
                return Result.Failure(new Error(403, "You do not own this job."));

            if (!job.IsActive)
                return Result.Failure(new Error(400, "Job is already closed."));

            job.Close();

            _unitOfWork.Jobs.Update(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
