using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Repositories
{
    public class RecruiterRepository : GenericRepository<Recruiter>, IRecruiterRepository
    {
        public RecruiterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Recruiter?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.ApplicationUserId == userId, cancellationToken);
        }
    }
}
