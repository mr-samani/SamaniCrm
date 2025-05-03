using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedDefaultUsers
    {
        public static async Task TrySeedAsync(
            ApplicationDbContext dbContext,
            ILogger<ApplicationDbInitializer> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            Console.WriteLine("Try seed Administor Role");
            // Default roles
            var administratorRole = new ApplicationRole(Roles.Administrator);

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
                await dbContext.SaveChangesAsync(); // ذخیره نقش
            }
            Console.WriteLine("Try seed Administor Role Permissions");
            // set role permissions
            var existingPermissionIds = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == administratorRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var allNewPermissions = await dbContext.Permissions
                .Where(p => !existingPermissionIds.Contains(p.Id))
                .Select(p => new RolePermission
                {
                    PermissionId = p.Id,
                    RoleId = administratorRole.Id
                })
                .ToListAsync();

            if (allNewPermissions.Any())
            {
                await dbContext.RolePermissions.AddRangeAsync(allNewPermissions);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Seeded {allNewPermissions.Count} new permissions for Administrator role.");
            }
            else
            {
                Console.WriteLine("No new permissions to seed for Administrator role.");
            }

            Console.WriteLine("Try seed Admin user");

            // Default users
            var administrator = new ApplicationUser
            {
                UserName = "samani",
                Email = "mr.samani1368@gmail.com",
                FirstName = "محمدرضا",
                LastName = "سامانی",
                FullName = "محمدرضا سامانی",
                Lang = "fa-IR",
                Address = "سامان",
                PhoneNumber = "09338972924",
                IsDeleted = false
            };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                var result = await userManager.CreateAsync(administrator, "123qwe");
                if (result.Succeeded)
                {
                    await dbContext.SaveChangesAsync(); // ذخیره کاربر
                    if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                    {
                        await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                    }
                }
                else
                {
                    // لاگ کردن خطاهای ایجاد کاربر
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("Error creating user: {Error}", error.Description);
                    }
                    throw new Exception("Failed to create user.");
                }
            }

            await dbContext.SaveChangesAsync();


        }
    }
}
