using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public required string RefreshTokenValue { get; set; }
        public bool Active { get; set; }
        public DateTime Expiration { get; set; }
        public bool Used { get; set; }
        public required string UserId { get; set; }
        public required string AccessToken { get; set; }
    }
}
