using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Infrastructure.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheService _cache;



        public UserPermissionService(ApplicationDbContext dbContext, ICacheService memoryCache)
        {
            _dbContext = dbContext;
            _cache = memoryCache;
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                return false;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return false;
            var cacheKey = CacheKeys.UserPermissions_ + userId;
            List<string>? permissions = await _cache.GetAsync<List<string>>(cacheKey);
            if (permissions == null)
            {
                var parsedUserId = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
                if (parsedUserId == Guid.Empty)
                    return false;

                var roleIds = await _dbContext.UserRoles
                    .Where(ur => ur.UserId == parsedUserId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                if (!roleIds.Any())
                    return false;

                permissions = await _dbContext.RolePermissions
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToListAsync();


                await _cache.SetAsync(cacheKey, permissions, TimeSpan.FromHours(8));
            }
            return permissions == null ? false : permissions.Contains(permission);
        }
    }

}
