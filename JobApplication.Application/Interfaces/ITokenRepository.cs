using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface ITokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> FindAsync(Expression<Func<RefreshToken, bool>> criteria);
    }
}
