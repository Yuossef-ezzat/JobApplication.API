using JobApplication.Application.Abstractions.ResultPattern;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Command.DeleteJob
{
    public class DeleteJobCommand : IRequest<Result>
    {
        public int JobId { get; set; }

        public DeleteJobCommand() { }

        public DeleteJobCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
