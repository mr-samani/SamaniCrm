using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Infrastructure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            services.AddLogging(logging => logging.AddConsole());

            // برای دسترسی به RoleManager و UserManager باید Identity رو هم رجیستر کنیم
            // اعمال تنظیمات سفارشی Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();



            services.AddScoped<ApplicationDbInitializer>();

            var provider = services.BuildServiceProvider();

            var context = provider.GetRequiredService<ApplicationDbContext>();

            // فقط موقع اجرای دستور Update-Database این فراخوانی میشه
            var initializer = provider.GetRequiredService<ApplicationDbInitializer>();
            initializer.SeedAsync().GetAwaiter().GetResult();


            
            return context;
        }
    }
}
