using JobApplication.Application.Abstractions.ResultPattern;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Command.UpdateJob
{
    public class UpdateJobCommand : IRequest<Result>
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
