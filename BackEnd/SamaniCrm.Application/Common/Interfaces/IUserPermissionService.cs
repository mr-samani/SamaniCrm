using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IUserPermissionService
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
    }

}
