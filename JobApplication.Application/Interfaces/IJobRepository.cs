using JobApplication.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<Job?> GetWithRecruiterByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Job>> GetActiveJobsWithRecruiterAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Job>> GetByRecruiterIdAsync(int recruiterId, CancellationToken cancellationToken = default);
    }
}
