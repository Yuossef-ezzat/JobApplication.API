using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _currentTransaction;

        public IJobRepository Jobs { get; }
        public IApplicationRepository Applications { get; }
        public ICandidateRepository Candidates { get; }
        public IRecruiterRepository Recruiters { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IJobRepository jobs,
            IApplicationRepository applications,
            ICandidateRepository candidates,
            IRecruiterRepository recruiters)
        {
            _context = context;
            Jobs = jobs;
            Applications = applications;
            Candidates = candidates;
            Recruiters = recruiters;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (IRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new GenericRepository<TEntity>(_context));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
