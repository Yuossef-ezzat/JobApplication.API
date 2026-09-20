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
    }
}
