using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IAuthService
    {
        public string GenerateAccessToken(IUser user);
        public Task<string> GenerateRefreshToken(IUser user, string accessToken,CancellationToken cancellationToken);
    }
}
