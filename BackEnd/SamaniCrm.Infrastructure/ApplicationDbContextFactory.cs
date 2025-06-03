using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace SamaniCrm.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../SamaniCrm.Api");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();

            // ✅ DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient
            );

            services.AddLogging(logging => logging.AddConsole());

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // 🔐 Password Rules
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // 👤 User Rules
                options.User.RequireUniqueEmail = true;

                // 🔒 Lockout
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                // 📧 Email confirmation
                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            services.AddScoped<ICurrentUserService, DummyCurrentUserService>();

            if (args.Contains("--seed"))
            {
                services.AddScoped<ApplicationDbInitializer>();
            }

            var provider = services.BuildServiceProvider();
            Console.WriteLine("Received args: " + string.Join(", ", args));
            Console.WriteLine(args.Contains("--seed"));
            // run only with "seed-database.bat"
            // dotnet ef database update -- --seed
            if (args.Contains("--seed"))
            {
                using var scope = provider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                var initializer = scope.ServiceProvider.GetService<ApplicationDbInitializer>();
                Console.WriteLine("initializer:" + (initializer != null));
                if (initializer != null)
                {
                    initializer.SeedAsync().GetAwaiter().GetResult();
                }
            }



            return provider.GetRequiredService<ApplicationDbContext>();
        }
    }

    public class DummyCurrentUserService : ICurrentUserService
    {
        public string? UserId => "MigrationUser"; // یا null هم میتونی بدی

        public string lang => "fa-IR";

        string ICurrentUserService.lang { get => lang; set => throw new NotImplementedException(); }
    }

}
