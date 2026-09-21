using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetJobApplications
{
    public class GetJobApplicationsQuery : IRequest<Result<IReadOnlyList<JobApplicationDetailsDto>>>
    {
        public int JobId { get; set; }

        public GetJobApplicationsQuery() { }

        public GetJobApplicationsQuery(int jobId)
        {
            JobId = jobId;
        }
    }
}
