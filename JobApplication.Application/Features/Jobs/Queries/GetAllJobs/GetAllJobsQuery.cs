using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace JobApplication.Application.Features.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQuery : IRequest<Result<IReadOnlyList<JobDto>>>
    {
        public string? SearchTerm { get; set; }
    }
}
