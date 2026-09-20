using JobApplication.Application.Interfaces;
using JobApplication.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IApplicationService, ApplicationService>();

            return services;
        }
    }
}
