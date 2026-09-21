using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace JobApplication.Application.Features.Jobs.Queries.GetRecruiterJobs
{
    public class GetRecruiterJobsQuery : IRequest<Result<IReadOnlyList<JobDto>>>
    {
    }
}
