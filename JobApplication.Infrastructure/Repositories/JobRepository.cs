using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;

namespace JobApplication.Infrastructure.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
