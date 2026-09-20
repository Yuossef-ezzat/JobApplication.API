using JobApplication.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationService
    {
        Task<int> ApplyAsync(ApplyJobRequest request, CancellationToken cancellationToken = default);
        Task CancelAsync(int applicationId, CancellationToken cancellationToken = default);
    }
}
