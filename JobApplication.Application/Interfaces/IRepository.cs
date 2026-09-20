using JobApplication.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetAll();
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
