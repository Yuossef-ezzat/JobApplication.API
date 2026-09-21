using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetApplicationById
{
    public class GetApplicationByIdQuery : IRequest<Result<JobApplicationDetailsDto>>
    {
        public int ApplicationId { get; set; }

        public GetApplicationByIdQuery() { }

        public GetApplicationByIdQuery(int applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
