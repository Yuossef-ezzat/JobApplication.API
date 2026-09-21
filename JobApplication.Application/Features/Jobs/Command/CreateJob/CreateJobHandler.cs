using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using MediatR;

namespace JobApplication.Application.Features.Job.Command.CreateJob
{
    public class CreateJobHandler : IRequestHandler<CreateJobCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateJobHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<Result<int>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null)
                return Result<int>.Failure(new Error(403, "Recruiter profile not found."));

            var job = new Domain.Entities.Job
            {
                Title = request.Title,
                Description = request.Description,
                IsActive = true,
                RecruiterId = recruiter.Id
            };

            await _unitOfWork.Jobs.InsertAsync(job, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(job.Id);
        }
    }
}
