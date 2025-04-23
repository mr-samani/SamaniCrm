using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Users.Queries
{

    public class UserPermissionsQueryHandler : IRequestHandler<UserPermissionsQuery, Dictionary<string, bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDbContext _context;

        public UserPermissionsQueryHandler(IDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Dictionary<string, bool>> Handle(UserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var rolePermissions = from ur in _context.UserRoles
                                  join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                                  where ur.UserId == userId
                                  select new
                                  {
                                      rp.Permission.Name,
                                      rp.IsGranted
                                  };

            var permissionsList = await rolePermissions.ToListAsync(cancellationToken);

            var result = permissionsList
                .GroupBy(x => x.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Any(x => x.IsGranted) // اگر حتی یکی true باشه، پرمیشن true
                );

            return result;
            // پرمیژن های مستقیم خود کاربر
            //var userPermissions = from up in _context.UserPermissions
            //                      where up.UserId == userId
            //                      select new { up.PermissionId, up.IsGranted };

            // جمع بندی با در نظر گرفتن اولویت User
            //var allPermissions = await _context.Permissions.ToListAsync();

            //var granted = allPermissions
            //    .Where(p =>
            //        userPermissions.Any(up => up.PermissionId == p.Id && up.IsGranted) ||
            //        (!userPermissions.Any(up => up.PermissionId == p.Id) && rolePermissions.Contains(p.Id))
            //    ).ToList();

            //return granted; 
        }
    }
}
