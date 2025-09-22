using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Permissions;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILocalizer L;

        public RolePermissionService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext,
            ILocalizer l)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            L = l;
        }
   

        public async Task<List<RolePermissionsDTO>> GetRolePermissionsAsyc(Guid roleId)
        {

            List<FlatPermission> allPermissions = PermissionsHelper.GetAllPermissions();

            var rolePermissionNames = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();

            // var tree = BuildPermissionTree2(allPermissions);
            var tree = BuildPermissionTree(allPermissions, rolePermissionNames);
            return tree;
        }


        private List<RolePermissionsDTO> BuildPermissionTree(
                List<FlatPermission> allPermissions,
                List<string> selectedPermissionNames)
        {
            var root = new List<RolePermissionsDTO>();

            foreach (var permission in allPermissions)
            {
                string[] parts = permission.Value.Replace("Permissions.", "").Split('.').Where(w => w.Trim() != "").ToArray();
                // string[] parts = permission.Value.Split('.').Where(w => w.Trim() != "").ToArray();
                AddToTree(root, parts, 0, permission.Value, permission.LocalizeKey, selectedPermissionNames);
            }

            return root;
        }

        private void AddToTree(
                         List<RolePermissionsDTO> nodes,
                         string[] parts,
                         int level,
                         string fullName,
                         string? displayName,
                         List<string> selectedPermissionNames)
        {
            var currentPart = parts[level];
            var currentFullName = string.Join('.', parts.Take(level + 1));

            var existingNode = nodes.FirstOrDefault(n => n.Name == currentFullName);
            if (existingNode == null)
            {
                existingNode = new RolePermissionsDTO
                {
                    Name = currentFullName,
                    DisplayName = displayName != null ? L[displayName] : currentFullName,
                    Selected = selectedPermissionNames.Contains(fullName),
                    Children = []
                };
                nodes.Add(existingNode);
            }

            if (level != parts.Length - 1)
            {
                AddToTree(existingNode.Children, parts, level + 1, fullName, displayName, selectedPermissionNames);
            }
        }





    }

}
