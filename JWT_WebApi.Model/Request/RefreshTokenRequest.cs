using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Model.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string ExpiredToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

    }
}
