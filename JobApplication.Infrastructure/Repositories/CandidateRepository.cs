using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Repositories
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Candidate?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ApplicationUserId == userId, cancellationToken);
        }
    }
}
