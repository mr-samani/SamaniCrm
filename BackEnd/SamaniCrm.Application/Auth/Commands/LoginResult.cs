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
        DateTime AccessTokenExpiration,
        DateTime RefreshTokenExpiration,
        string UserId,
        string UserName,
        string? Email,
        string FirstName,
        string LastName,
        string? ProfilePicture,
        string[] Roles
       );
}
