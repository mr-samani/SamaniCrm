using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<string>> GetRolePermissionsAsyc(Guid RoleId);

        Task<List<string>> GetPermissionsForUserAsync(Guid userId);
        Task<bool> HasPermissionAsync(Guid userId, string permissionName);
    }
}
