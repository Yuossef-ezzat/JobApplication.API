using JobApplication.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IRecruiterRepository : IRepository<Recruiter>
    {
        Task<Recruiter?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
