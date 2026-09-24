using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; set; }
        public string HashToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked => (RevokedAt is not null);
        public bool IsExpired => (ExpirationDate <= DateTime.UtcNow);
    }
}
