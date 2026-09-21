using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public class JobCandidateApplication : BaseEntity
    {
        public int CandidateId { get; set; }
        [ForeignKey(nameof(CandidateId))]
        public Candidate Candidate { get; set; } = null!;
        public int JobId { get; set; }
        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; } = null!;
        public JobApplicationStatus JobApplicationStatus { get; set; }
        public DateTime AppliedAt { get; set; }

        public void Cancel()
        {
            if (JobApplicationStatus is JobApplicationStatus.Accepted or JobApplicationStatus.Rejected)
            {
                throw new InvalidOperationException("Application cannot be cancelled if its status is Accepted or Rejected.");
            }

            if (JobApplicationStatus == JobApplicationStatus.Cancelled)
            {
                throw new InvalidOperationException("Application is already cancelled.");
            }

            JobApplicationStatus = JobApplicationStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(JobApplicationStatus newStatus)
        {
            if (JobApplicationStatus == JobApplicationStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot update status of a cancelled application.");
            }

            if (JobApplicationStatus is JobApplicationStatus.Accepted or JobApplicationStatus.Rejected)
            {
                throw new InvalidOperationException($"Application has reached a finalized status ({JobApplicationStatus}) and cannot be updated.");
            }

            if (newStatus == JobApplicationStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot set status to Cancelled via UpdateStatus. Candidates cancel applications.");
            }

            bool isValid = (JobApplicationStatus, newStatus) switch
            {
                (JobApplicationStatus.Applied, JobApplicationStatus.UnderReview) => true,
                (JobApplicationStatus.UnderReview, JobApplicationStatus.InterView) => true,
                (JobApplicationStatus.InterView, JobApplicationStatus.Accepted) => true,
                (JobApplicationStatus.InterView, JobApplicationStatus.Rejected) => true,
                _ => false
            };

            if (!isValid)
            {
                throw new InvalidOperationException($"Invalid status transition from '{JobApplicationStatus}' to '{newStatus}'. Allowed forward flow: Applied -> UnderReview -> InterView -> Accepted or Rejected.");
            }

            JobApplicationStatus = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
