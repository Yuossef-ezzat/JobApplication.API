using JobApplication.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        Task<Candidate?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
