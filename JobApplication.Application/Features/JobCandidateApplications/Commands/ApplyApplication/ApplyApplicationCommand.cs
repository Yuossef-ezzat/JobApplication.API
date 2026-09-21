using JobApplication.Application.Abstractions.ResultPattern;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.JobCandidateApplication.Commands.ApplyApplication
{
    public class ApplyApplicationCommand : IRequest<Result<int>>
    {
        public int JobId { get; set; }
    }
}
