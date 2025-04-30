using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public RolePermissionService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user);

            var permissions = new List<string>();
            foreach (var role in roles)
            {
                var appRole = await _roleManager.FindByNameAsync(role);
                if (appRole != null)
                {
                    // فرض: permissionها در جدول خاصی ذخیره شده و به نقش‌ها map شدن
                    permissions.AddRange(await GetRolePermissionsAsyc(appRole.Id));
                }
            }

            return permissions.Distinct().ToList();
        }

      

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
        {
            var permissions = await GetPermissionsForUserAsync(userId);
            return permissions.Contains(permissionName);
        }

        public async Task<List<string>> GetRolePermissionsAsyc(Guid RoleId)
        {
            throw new NotImplementedException();
            //var result=await _dbContext.RolePermissions.Where(x=>x.RoleId == RoleId)
            //    .Select(s=>s.Permission.Name).ToListAsync();
            //return result;
        }
    }

}
