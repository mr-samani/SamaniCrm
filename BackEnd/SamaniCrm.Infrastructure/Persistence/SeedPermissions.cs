using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SamaniCrm.Core.AppPermissions;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedPermissions
    {


        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Try seed permission data");

            var permissions = typeof(AppPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => new
                {
                    localizeKey = f.Name,
                    value = f.GetRawConstantValue()?.ToString(),
                })
                .Where(p => !string.IsNullOrEmpty(p.value))
                .ToList();

            var existingPermissions = dbContext.Permissions.Select(p => p.Name).ToHashSet();

            var newPermissions = permissions
                .Where(p => !existingPermissions.Contains(p.value!))
                .Select(p => new Permission { Name = p.value!, LocalizeKey = p.localizeKey })
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
