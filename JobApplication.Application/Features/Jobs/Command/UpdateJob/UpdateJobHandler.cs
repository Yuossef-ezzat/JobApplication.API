using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Command.UpdateJob
{
    public class UpdateJobHandler : IRequestHandler<UpdateJobCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateJobHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result.Failure(new Error(404, "Job not found."));

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || job.RecruiterId != recruiter.Id)
                return Result.Failure(new Error(403, "You do not own this job."));

            if (!job.IsActive)
                return Result.Failure(new Error(400, "Cannot update a closed job."));

            job.Update(request.Title, request.Description);

            _unitOfWork.Jobs.Update(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
