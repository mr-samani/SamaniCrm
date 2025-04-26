using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        public string GenerateAccessToken(Guid userId, string userName, IList<string> roles);
        public Task<string> GenerateRefreshToken(Guid userId, string accessToken);
    }
}
