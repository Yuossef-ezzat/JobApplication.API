using Hangfire;
using JobApplication.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class HangfireService : IBackgroundJob
    {
        private readonly IBackgroundJobClient _background;
        public HangfireService(IBackgroundJobClient background)
        {
            _background = background;
        }
        public void EnqueueJob<T>(Expression<Action<T>> methodCall)
        {
            _background.Enqueue(methodCall);
        }
    }
}
