using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services;

public class UserPermissionService : IUserPermissionService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;



    public UserPermissionService(IApplicationDbContext dbContext, 
        ICacheService memoryCache,
        ICurrentUserService currentUserService, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager)
    {
        _dbContext = dbContext;
        _cache = memoryCache;
        _currentUser = currentUserService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return false;

        Guid? userId = _currentUser.UserId;
        if (userId.HasValue == false)
        {
            return false;
        }
        List<string> permissions = await GetUserPermissionsAsync(userId.Value, CancellationToken.None);
        return permissions == null ? false : permissions.Contains(permission);
    }



    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserPermissions_ + userId;
        List<string>? permissions = await _cache.GetAsync<List<string>>(cacheKey);
        if (permissions == null)
        {

            List<Guid> roleIds = await _userManager.Users
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Roles.Select(r => r.Id))
                .ToListAsync(cancellationToken);

            //var roleIds = await _dbContext.UserRoles
            //    .Where(ur => ur.UserId == userId)
            //    .Select(ur => ur.RoleId)
            //    .ToListAsync();

            if (!roleIds.Any())
                return [];

            permissions = await _dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();


            await _cache.SetAsync(cacheKey, permissions, TimeSpan.FromHours(8));
        }
        return permissions;
    }

}
