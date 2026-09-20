using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public JobService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<int> CreateAsync(CreateJobDto createJobDto, CancellationToken cancellationToken = default)
        {
            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null)
                throw new UnauthorizedAccessException("Recruiter profile not found.");

            var job = new Job
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = true,
                RecruiterId = recruiter.Id
            };

            await _unitOfWork.Jobs.InsertAsync(job, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return job.Id;
        }

        public async Task CloseAsync(int jobId, CancellationToken cancellationToken = default)
        {
            var job = await _unitOfWork.Jobs.GetByIdAsync(jobId, cancellationToken);
            if (job == null)
                throw new KeyNotFoundException("Job not found.");

            var recruiter = await _unitOfWork.Recruiters.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (recruiter == null || job.RecruiterId != recruiter.Id)
                throw new UnauthorizedAccessException("You do not own this job.");

            job.Close();

            _unitOfWork.Jobs.Update(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
