using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domain.Entities
{
    public class Candidate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string CvUrl { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
