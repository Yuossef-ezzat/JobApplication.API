using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Queries.GetJobById
{
    public class GetJobByIdQuery : IRequest<Result<JobDetailsDto>>
    {
        public int JobId { get; set; }

        public GetJobByIdQuery() { }

        public GetJobByIdQuery(int jobId)
        {
            JobId = jobId;
        }
    }
}
