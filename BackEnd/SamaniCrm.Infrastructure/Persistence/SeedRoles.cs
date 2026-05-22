using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Persistence;

public class SeedRoles
{
    public static async Task TrySeedAsync(
      ApplicationDbContext dbContext,
      ILogger<ApplicationDbInitializer> logger,
      RoleManager<ApplicationRole> roleManager)
    {
        Console.WriteLine("Try seed Roles");
        var allRoles = typeof(AppRoles).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                           .Where(f => f.FieldType == typeof(string))
                           .Select(s => s.Name)
                           .ToList();

        foreach (string role in allRoles)
        {
            // 1. ایجاد نقش های هاست در صورت عدم وجود
            var r = await roleManager.FindByNameAsync(role);
            if (r == null)
            {
                r = new ApplicationRole(role);
                switch (role)
                {
                    case AppRoles.SysAdmin:
                        r.Level = 0;
                        break;
                    case AppRoles.TenantAdministrator:
                        r.Level = 1;
                        break;
                    default:
                        r.Level = 2;
                        break;
                }
                r.IsSystem = true;
                var roleResult = await roleManager.CreateAsync(r);
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        logger.LogError("Error creating role: {Error}", error.Description);
                    }
                    throw new Exception("Failed to create role:" + role);
                }
                Console.WriteLine("Role: \"" + role + "\" Added.");

                await dbContext.SaveChangesAsync();
            }
        }
        Console.WriteLine("Try seed Administrator Role Permissions");


        var administratorRole = await roleManager.FindByNameAsync(AppRoles.SysAdmin);

        if (administratorRole == null)
        {
            throw new Exception("Adminstrator role not created in DB!");
        }
        // Add all permissions to super admin
        // 2. اضافه کردن پرمیشن های جدید به نقش ادمین
        var existingPermissionIds = await dbContext.RolePermissions
            .Where(rp => rp.RoleId == administratorRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var newPermissions = await dbContext.Permissions
            .Where(p => !existingPermissionIds.Contains(p.Id))
            .Select(p => new RolePermission
            {
                RoleId = administratorRole.Id,
                PermissionId = p.Id
            })
            .ToListAsync();

        if (newPermissions.Any())
        {
            await dbContext.RolePermissions.AddRangeAsync(newPermissions);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Seeded {newPermissions.Count} new permissions for Administrator role.");
        }
        else
        {
            Console.WriteLine("No new permissions to seed for Administrator role.");
        }


    }
}
