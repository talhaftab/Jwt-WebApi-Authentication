using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Model
{
    public class UserRefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive
        {
            get
            {
                return ExpirationDate < DateTime.UtcNow;
            }
        }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsInvalidated { get; set; }

        [ForeignKey("Id")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
