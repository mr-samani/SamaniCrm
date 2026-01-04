using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class AuthResponseDTO
    {
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Token { get; set; }
    }
}
