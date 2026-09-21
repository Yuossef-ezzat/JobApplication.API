using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace JobApplication.Application.Features.JobCandidateApplications.Queries.GetCandidateApplications
{
    public class GetCandidateApplicationsQuery : IRequest<Result<List<CandidateApplicationDto>>>
    {
    }
}
