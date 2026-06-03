using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Helpers;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedPermissions
    {


        public static async Task TrySeedAsync(TenantDbContext dbContext)
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
