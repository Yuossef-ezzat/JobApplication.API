using JobApplication.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    /// <summary>
    /// Recurring Hangfire job that automatically closes any active Job
    /// that has been open for longer than <see cref="StaleAfterDays"/> days.
    ///
    /// How RecurringJob.AddOrUpdate works
    /// ────────────────────────────────────
    /// Hangfire stores a record in its SQL tables for each recurring job.
    /// When <c>RecurringJob.AddOrUpdate(jobId, () => ..., cronExpression)</c> is called:
    ///   1. If a record with <paramref name="jobId"/> does NOT exist → it is created.
    ///   2. If the record already exists → the schedule / method is UPDATED in-place.
    /// The job fires according to the Cron expression and is retried automatically on failure.
    ///
    /// What is a Cron expression?
    /// ───────────────────────────
    /// A Cron expression is a compact string that describes a repeating schedule:
    ///
    ///     ┌─────── minute  (0-59)
    ///     │ ┌───── hour    (0-23)
    ///     │ │ ┌─── day of month (1-31)
    ///     │ │ │ ┌─ month   (1-12)
    ///     │ │ │ │ ┌ day of week (0-6, Sun=0)
    ///     │ │ │ │ │
    ///     0 2 * * *   → every day at 02:00
    ///     0 0 * * 0   → every Sunday at midnight
    ///     */5 * * * * → every 5 minutes
    ///
    /// Hangfire ships <see cref="Cron"/> helper constants (Cron.Daily, Cron.Weekly, etc.)
    /// and also accepts a raw cron string.
    /// </summary>
    public class AutoCloseJobsService
    {
        /// <summary>Jobs open longer than this many days are considered stale.</summary>
        private const int StaleAfterDays = 30;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AutoCloseJobsService> _logger;

        public AutoCloseJobsService(IUnitOfWork unitOfWork, ILogger<AutoCloseJobsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Entry point called by Hangfire on each scheduled trigger.
        /// Finds all active jobs older than <see cref="StaleAfterDays"/> days and closes them.
        /// </summary>
        public async Task AutoCloseStaleJobsAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-StaleAfterDays);

            _logger.LogInformation(
                "[AutoCloseJobsService] Scanning for active jobs created before {Cutoff} (older than {Days} days).",
                cutoff, StaleAfterDays);

            var staleJobs = await _unitOfWork.Jobs.GetStaleActiveJobsAsync(cutoff);

            if (staleJobs.Count == 0)
            {
                _logger.LogInformation("[AutoCloseJobsService] No stale jobs found. Nothing to close.");
                return;
            }

            int closedCount = 0;
            foreach (var job in staleJobs)
            {
                try
                {
                    job.Close();   // Uses the domain method — sets IsActive = false, UpdatedAt = UtcNow
                    closedCount++;
                    _logger.LogInformation(
                        "[AutoCloseJobsService] Auto-closed Job #{JobId} ('{Title}'), created {CreatedAt:yyyy-MM-dd}.",
                        job.Id, job.Title, job.CreatedAt);
                }
                catch (InvalidOperationException ex)
                {
                    // Job was already closed between the query and the loop — safe to skip
                    _logger.LogWarning(
                        "[AutoCloseJobsService] Skipped Job #{JobId}: {Reason}", job.Id, ex.Message);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "[AutoCloseJobsService] Done. {Closed} job(s) auto-closed out of {Total} stale candidate(s).",
                closedCount, staleJobs.Count);
        }
    }
}
