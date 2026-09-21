using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Repositories
{
    public class ApplicationRepository : GenericRepository<JobCandidateApplication>, IApplicationRepository
    {
        public ApplicationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int candidateId, int jobId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(a => a.CandidateId == candidateId && a.JobId == jobId, cancellationToken);
        }

        public async Task<IReadOnlyList<JobCandidateApplication>> GetByCandidateIdWithDetailsAsync(int candidateId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Job)
                    .ThenInclude(j => j.Recruiter)
                .Include(a => a.Candidate)
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<JobCandidateApplication>> GetByJobIdWithCandidateAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<JobCandidateApplication?> GetWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(a => a.Job)
                    .ThenInclude(j => j.Recruiter)
                .Include(a => a.Candidate)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
    }
}
