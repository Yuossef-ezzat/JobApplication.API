using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces.IServices
{
    public interface IBackgroundJob
    {
        void EnqueueJob<T>(Expression<Action<T>> methodCall);
    }
}
