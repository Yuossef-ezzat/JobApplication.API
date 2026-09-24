using JobApplication.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IJobRepository Jobs { get; }
        IApplicationRepository Applications { get; }
        ICandidateRepository Candidates { get; }
        IRecruiterRepository Recruiters { get; }
        ITokenRepository Tokens { get; }

        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
