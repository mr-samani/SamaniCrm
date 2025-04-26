using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Commands
{
    public record LoginResult(
         string AccessToken,
         string RefreshToken,
         Guid UserId,
         string? UserName,
         string? Email,
         string FullName,
         string? ProfilePicture,
         string[] Roles
        );
}
