
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace SamaniCrm.Infrastructure.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // ✅ تنظیمات پیشرفته Identity
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
            .AddDefaultTokenProviders();

            // ✅ Cookie Auth Settings
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.LoginPath = "/auth/login";
                options.AccessDeniedPath = "/auth/access-denied";
            });

            // ✅ IdentityServer
            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential(); // ❗ در Production با Certificate جایگزین شود

            // ✅ Identity API Endpoints (برای minimal APIs در .NET 7+)
            // services.AddIdentityApiEndpoints<ApplicationUser>();




            return services;
        }


    }
}
