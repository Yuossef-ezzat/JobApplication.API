using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Command.DeleteJob
{
    public class DeleteJobHandler : IRequestHandler<DeleteJobCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteJobHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
                return Result.Failure(new Error(404, "Job not found."));

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || job.RecruiterId != recruiter.Id)
                return Result.Failure(new Error(403, "You do not own this job."));

            // Perform Soft Delete
            job.SoftDelete();

            _unitOfWork.Jobs.Update(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
