using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateApplicationStatus
{
    public class UpdateApplicationStatusCommand : IRequest<Result>
    {
        public int ApplicationId { get; set; }
        public JobApplicationStatus Status { get; set; }
    }
}
