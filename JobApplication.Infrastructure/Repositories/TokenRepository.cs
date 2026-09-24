using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Repositories
{
    public class TokenRepository : GenericRepository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<RefreshToken> FindAsync(Expression<Func<RefreshToken, bool>> criteria)
        {
            var entity = await _dbSet.Where(criteria).FirstOrDefaultAsync();
            return entity;
        }
    }
}
