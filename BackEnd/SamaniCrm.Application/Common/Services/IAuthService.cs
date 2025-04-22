using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Common.Services
{
    public interface IAuthService
    {
        public string GenerateAccessToken(ApplicationUser user);
        public Task<string> GenerateRefreshToken(ApplicationUser user, string accessToken);
    }
}
