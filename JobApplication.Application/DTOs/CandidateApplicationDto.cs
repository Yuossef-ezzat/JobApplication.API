using JobApplication.Domain.Enums;
using System;

namespace JobApplication.Application.DTOs
{
    public class CandidateApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; }
        public DateTime? AppliedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
