using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Core.Shared.Permissions;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedPermissions
    {


        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Try seed permission data");

            var permissions = PermissionsHelper.GetAllPermissions();

            var existingPermissions = dbContext.Permissions
                .Select(p => p.Name)
                .IgnoreQueryFilters()
                .ToHashSet();

            var newPermissions = permissions
                .Where(p => !existingPermissions.Contains(p.Value!))
                .Select(p => new Permission { Name = p.Value!, LocalizeKey = p.LocalizeKey })
                .ToList();
            Console.WriteLine("New Permissions Count: " + newPermissions.Count);
            if (newPermissions.Any())
            {
                await dbContext.Permissions.AddRangeAsync(newPermissions);
                await dbContext.SaveChangesAsync();
            }




        }
    }
}
