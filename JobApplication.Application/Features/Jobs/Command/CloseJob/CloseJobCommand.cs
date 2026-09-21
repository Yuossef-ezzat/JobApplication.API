using JobApplication.Application.Abstractions.ResultPattern;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.Command.CloseJob
{
    public class CloseJobCommand : IRequest<Result>
    {
        public int JobId { get; set; }
    }
}
