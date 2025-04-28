using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class UserDetailsResponseDTO:UserResponseDTO
    {
        public IList<string> Roles { get; set; } = [];
    }
}
