using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginResult
    {
        public UserDTO User { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];
        public List<string> Permissions { get; set; } = [];

        public bool EnableTwoFactor { get; set; } = false;
        public TwoFactorTypeEnum TwoFactorType { get; set; }
    }

}
