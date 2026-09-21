using JobApplication.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationRepository : IRepository<JobCandidateApplication>
    {
        Task<bool> ExistsAsync(int candidateId, int jobId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobCandidateApplication>> GetByCandidateIdWithDetailsAsync(int candidateId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobCandidateApplication>> GetByJobIdWithCandidateAsync(int jobId, CancellationToken cancellationToken = default);
        Task<JobCandidateApplication?> GetWithDetailsByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
