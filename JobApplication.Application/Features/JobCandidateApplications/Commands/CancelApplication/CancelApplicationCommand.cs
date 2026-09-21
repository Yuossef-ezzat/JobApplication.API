using JobApplication.Application.Abstractions.ResultPattern;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplication.Commands.CancelApplication
{
    public class CancelApplicationCommand : IRequest<Result>
    {
        public int ApplicationId { get; set; }
    }
}
