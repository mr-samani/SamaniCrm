using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Persistence;
using System.Linq.Dynamic.Core;

public static class SeedDefaultUsers
{
    public static async Task TrySeedAsync(
        ApplicationDbContext dbContext,
        ILogger<ApplicationDbInitializer> logger,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {


        Console.WriteLine("Try seed Admin user");
        var administratorRole = await roleManager.FindByNameAsync(AppRoles.Administrator);
        if (administratorRole == null)
        {
            throw new Exception("Administrator role not found on DB!");
        }


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

