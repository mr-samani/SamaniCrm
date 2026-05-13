using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

public static class SeedDefaultUsers
{
    public static async Task TrySeedAsync(
        ApplicationDbContext dbContext,
        ILogger<ApplicationDbInitializer> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        Console.WriteLine("Try seed Roles");
        var allRoles = typeof(Roles).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                           .Where(f => f.FieldType == typeof(string))
                           .Select(s => s.Name)
                           .ToList();

        foreach (string role in allRoles)
        {
            // 1. ایجاد نقش ادمین در صورت عدم وجود
            var r = await roleManager.FindByNameAsync(role);
            if (r == null)
            {
                r = new ApplicationRole(role);
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


        var administratorRole = await roleManager.FindByNameAsync(Roles.Administrator);

        if (administratorRole == null)
        {

            throw new Exception("Adminstrator role not created in DB!");
        }

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


        Console.WriteLine("Try seed Admin user");

        // 3. ایجاد کاربر پیش فرض ادمین در صورت عدم وجود
        var adminUser = await userManager.FindByNameAsync("samani");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "samani",
                Email = "mr.samani1368@gmail.com",
                FirstName = "محمدرضا",
                LastName = "سامانی",
                FullName = "محمدرضا سامانی",
                Lang = "fa-IR",
                Address = "سامان",
                PhoneNumber = "09338972924",
            };

            var userResult = await userManager.CreateAsync(adminUser, "123qwe");
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                {
                    logger.LogError("Error creating user: {Error}", error.Description);
                }
                throw new Exception("Failed to create administrator user.");
            }

            await userManager.AddToRoleAsync(adminUser, administratorRole.Name!);
        }

        await dbContext.SaveChangesAsync();
    }
}

