using JobApplication.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationRepository : IRepository<JobCandidateApplication>
    {
        Task<bool> ExistsAsync(int candidateId, int jobId, CancellationToken cancellationToken = default);
    }
}
