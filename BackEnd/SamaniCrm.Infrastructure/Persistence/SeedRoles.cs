using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Identity;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Persistence;

public class SeedRoles
{
    public static async Task TrySeedAsync(
        TenantDbContext hostDbContext,
        TenantDbContext dbContext,
        Guid? tenantId,
        ILogger<ApplicationDbInitializer> logger)
    {
        Console.WriteLine("Try seeding Roles...");

        // اصلاح: استفاده از GetValue برای گرفتن مقدار واقعی رشته
        var allRoles = typeof(AppRoles).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                           .Where(f => f.FieldType == typeof(string))
                           .Select(f => (string)f.GetValue(null)!)
                           .ToList();

        foreach (string roleName in allRoles)
        {
            // 1. چک کردن وجود نقش
            var roleExists = await dbContext.Roles
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Name == roleName && x.TenantId == tenantId);

            if (!roleExists)
            {
                var r = new ApplicationRole(roleName)
                {
                    TenantId = tenantId,
                    IsSystem = true,
                    Level = roleName switch
                    {
                        AppRoles.SysAdmin => 0,
                        AppRoles.TenantAdministrator => 1,
                        _ => 2
                    }
                };

                await dbContext.Roles.AddAsync(r);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Role: \"{roleName}\" added.");
            }
        }

        // فراخوانی متدهای اصلاح شده (که در پیام‌های قبلی بررسی کردیم)
        await SeedSysAdminRolePermissions(dbContext, tenantId);
        await SeedTenantAdminRolePermissions(hostDbContext,dbContext, tenantId);

        Console.WriteLine("Roles and permissions seeding completed.");
    }




    public static async Task SeedSysAdminRolePermissions(TenantDbContext dbContext, Guid? tenantId)
    {
        Console.WriteLine("Try seeding Administrator Role Permissions");

        // ۱. پیدا کردن نقش مدیر سیستم
        var sysAdminRole = await dbContext.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Name == AppRoles.SysAdmin && x.TenantId == tenantId);

        if (sysAdminRole == null)
        {
            throw new Exception($"Administrator role not found for Tenant: {tenantId}");
        }

        // ۲. دریافت لیست پرمیشن‌هایی که قبلاً برای این نقش ثبت شده‌اند
        var existingPermissionIds = await dbContext.RolePermissions
            .IgnoreQueryFilters()
            .Where(rp =>rp.TenantId == tenantId &&  rp.RoleId == sysAdminRole.Id) // TenantId در RoleId نهفته است
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        // ۳. پیدا کردن پرمیشن‌هایی که هنوز برای این نقش ثبت نشده‌اند
        // استفاده از ToList قبل از Select برای جلوگیری از خطای اجرای کوئری
        var allPermissionIds = await dbContext.Permissions
            .Select(p => p.Id)
            .ToListAsync();

        var missingPermissionIds = allPermissionIds
            .Where(id => !existingPermissionIds.Contains(id))
            .ToList();

        if (missingPermissionIds.Any())
        {
            var newPermissions = missingPermissionIds.Select(pId => new RolePermission
            {
                TenantId = tenantId,
                RoleId = sysAdminRole.Id,
                PermissionId = pId
            }).ToList();

            await dbContext.RolePermissions.AddRangeAsync(newPermissions);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Seeded {newPermissions.Count} new permissions for Administrator role.");
        }
        else
        {
            Console.WriteLine("No new permissions to seed for Administrator role.");
        }
    }



    /// <summary>
    /// کپی کردن دسترسی های مدیر بهره بردار از روی هاست بر روی بهره بردار 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task SeedTenantAdminRolePermissions(TenantDbContext hostDbContext, TenantDbContext dbContext, Guid? tenantId)
    {
        if (!tenantId.HasValue) return;

        Console.WriteLine("Try seeding Tenant Administrator Role Permissions");

        // ۱. پیدا کردن نقش مدیر در هاست
        var hostTenantAdminRole = await hostDbContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Name == AppRoles.TenantAdministrator && x.TenantId == null);

        if (hostTenantAdminRole == null)
            throw new Exception("Tenant Administrator role does not exist for host!");

        // ۲. پیدا کردن نقش مدیر در Tenant
        var tenantAdminRole = await dbContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Name == AppRoles.TenantAdministrator && x.TenantId == tenantId);

        if (tenantAdminRole == null)
            throw new Exception($"Tenant Administrator role does not exist for Tenant: {tenantId}");

        // ۳. گرفتن نام (Name) پرمیشن‌هایی که در هاست به این نقش داده شده
        // ما به جای ID، نام‌ها را می‌گیریم تا بتوانیم در Tenant دنبالشان بگردیم
        var hostPermissionNames = await hostDbContext
            .RolePermissions
            .IgnoreQueryFilters() // حتماً اینجا ignore کنید تا بتوانید دسترسی‌ها را ببینید
            .Where(rp => rp.RoleId == hostTenantAdminRole.Id)
            .Join(hostDbContext.Permissions,
                  rp => rp.PermissionId,
                  p => p.Id,
                  (rp, p) => p.Name) // فقط نام‌ها را برمی‌گردانیم
            .ToListAsync();

        // ۴. گرفتن پرمیشن‌های موجود در Tenant (با استفاده از نام برای تطبیق)
        var existingTenantPermissions = await dbContext.RolePermissions
            .IgnoreQueryFilters()
            .Where(rp => rp.RoleId == tenantAdminRole.Id)
            .Join(dbContext.Permissions,
                  rp => rp.PermissionId,
                  p => p.Id,
                  (rp, p) => p.Name)
            .ToListAsync();

        // ۵. پیدا کردن پرمیشن‌های Tenant که هنوز به این نقش داده نشده‌اند
        // ابتدا باید IDهای واقعی پرمیشن‌های Tenant را بر اساس نام پیدا کنیم
        var tenantPermissionMap = await dbContext.Permissions
            .IgnoreQueryFilters()
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();

        var newPermissionsToAssign = hostPermissionNames
            .Where(name => !existingTenantPermissions.Contains(name)) // پرمیشن‌هایی که در هاست هست ولی در Tenant نیست
            .Select(name => new
            {
                Id = tenantPermissionMap.FirstOrDefault(x => x.Name == name)?.Id,
                Name = name
            })
            .Where(x => x.Id.HasValue) // فقط اگر در Tenant واقعاً پرمیشنی با این نام وجود داشت
            .Select(x => new RolePermission
            {
                TenantId = tenantId,
                RoleId = tenantAdminRole.Id,
                PermissionId = x.Id.Value // حالا ID واقعیِ پرمیشنِ مربوط به این Tenant را استفاده می‌کنیم
            })
            .ToList();

        // ۶. درج دسترسی‌های جدید
        if (newPermissionsToAssign.Any())
        {
            await dbContext.RolePermissions.AddRangeAsync(newPermissionsToAssign);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Seeded {newPermissionsToAssign.Count} new permissions for Tenant {tenantId} using Tenant-specific IDs.");
        }
        else
        {
            Console.WriteLine("No new permissions to seed.");
        }
    }
}
