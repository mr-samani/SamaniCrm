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
            Console.WriteLine("Start Seeding DataBase...");

            await SeedPermissions.TrySeedAsync(_context);
            // seeld localization must be after seed permissions
            await SeedLocalization.TrySeedAsync(_context);
            await SeedStaticMenus.TrySeedAsync(_context);
            await SeedDefaultUsers.TrySeedAsync(_context,_logger,_userManager,_roleManager);
            await SeedSecuritySettings.TrySeedAsync(_context);
            await SeedProductCategoriesFromFile.TrySeedAsync(_context);
            await SeedCurrencies.TrySeedAsync(_context);

            await SeedDataSources.TrySeedAsync(_context);
        }
    }


}
