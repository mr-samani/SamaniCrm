using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
        string? UserName { get; }
        IEnumerable<Claim> Claims { get; }
        bool HasPermission(string permission);
    }
}
