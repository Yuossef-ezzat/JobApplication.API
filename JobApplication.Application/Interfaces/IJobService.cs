using JobApplication.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IJobService
    {
        Task<int> CreateAsync(CreateJobDto createJobDto, CancellationToken cancellationToken = default);
        Task CloseAsync(int jobId, CancellationToken cancellationToken = default);
    }
}
