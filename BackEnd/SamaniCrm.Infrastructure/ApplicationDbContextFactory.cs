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

            // فقط اگه قصد Seed داریم (مثلاً موقع Update-Database) این خط اجرا بشه
            if (args.Contains("seed", StringComparer.OrdinalIgnoreCase))
            {
                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders()
                    .AddSignInManager();

                services.AddScoped<ApplicationDbInitializer>();

                var provider = services.BuildServiceProvider();
                var initializer = provider.GetRequiredService<ApplicationDbInitializer>();
                initializer.SeedAsync().GetAwaiter().GetResult();

                return provider.GetRequiredService<ApplicationDbContext>();
            }

            // حالت عادی - بدون اجرای Seed
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
