using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class InitialiserExtensions
    {
        public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
        {
            builder.UseAsyncSeeding(async (context, _, ct) =>
            {
                var initialiser = serviceProvider.GetRequiredService<ApplicationDbInitializer>();

                await initialiser.SeedAsync();
            });
        }

        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();

            await initialiser.InitialiseAsync();
        }
    }





    public class ApplicationDbInitializer
    {
        private readonly ILogger<ApplicationDbInitializer> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbInitializer(
            ILogger<ApplicationDbInitializer> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
            )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {

            await SeedLocalization.TrySeedAsync(_context);
            await SeedPermissions.TrySeedAsync(_context);

            // Default roles
            var administratorRole = new ApplicationRole(Roles.Administrator);

            if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
                await _context.SaveChangesAsync(); // ذخیره نقش
            }

            // Default users
            var administrator = new ApplicationUser
            {
                UserName = "samani",
                Email = "mr.samani1368@gmail.com",
                FirstName = "محمدرضا",
                LastName = "سامانی",
                FullName = "محمدرضا سامانی",
                Lang = "fa",
                Address = "سامان",
                PhoneNumber = "09338972924",
                IsDeleted = false
            };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                var result = await _userManager.CreateAsync(administrator, "123qwe");
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync(); // ذخیره کاربر
                    if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                    {
                        await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                    }
                }
                else
                {
                    // لاگ کردن خطاهای ایجاد کاربر
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: {Error}", error.Description);
                    }
                    throw new Exception("Failed to create user.");
                }
            }

            await _context.SaveChangesAsync();


            // ذخیره نهایی تغییرات
            // Default data
            // Seed, if necessary
            //if (!_context.TodoLists.Any())
            //{
            //    _context.TodoLists.Add(new TodoList
            //    {
            //        Title = "Todo List",
            //        Items =
            //    {
            //        new TodoItem { Title = "Make a todo list 📃" },
            //        new TodoItem { Title = "Check off the first item ✅" },
            //        new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
            //        new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
            //    }
            //    });

            // await _context.SaveChangesAsync();
            //}
        }
    }


}
