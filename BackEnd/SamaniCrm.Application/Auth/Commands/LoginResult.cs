using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginResult
    {
        public UserResponseDTO User { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];
    }

}
