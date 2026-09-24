using JobApplication.Application.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class EmailNotificationService(ILogger<EmailNotificationService> _logger) : INotificationService
    {
        public void SendNotification(int ApplicationId)
        {
            _logger.LogInformation($"Sending email notification for application ID: {ApplicationId} has been Canceled");
        }
    }
}
