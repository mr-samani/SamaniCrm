using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<RolePermissionsDTO>> GetRolePermissionsAsyc(Guid RoleId);

        Task<List<string>> GetPermissionsForUserAsync(Guid userId);
        Task<bool> PermissionAsync(Guid userId, string permissionName);
    }
}
