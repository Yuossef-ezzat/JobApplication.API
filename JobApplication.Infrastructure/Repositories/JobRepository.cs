using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Job?> GetWithRecruiterByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(j => j.Recruiter)
                .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Job>> GetActiveJobsWithRecruiterAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Include(j => j.Recruiter)
                .Where(j => j.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(j => j.Title.ToLower().Contains(term) || j.Description.ToLower().Contains(term));
            }

            return await query.OrderByDescending(j => j.Id).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Job>> GetByRecruiterIdAsync(int recruiterId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(j => j.Recruiter)
                .Where(j => j.RecruiterId == recruiterId)
                .OrderByDescending(j => j.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
