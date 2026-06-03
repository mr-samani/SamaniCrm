using Duende.IdentityServer;
using Duende.IdentityServer.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Settings;
using SamaniCrm.Infrastructure.DbContexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            OIDCSettings oidcSettings = configuration.GetSection("OIDC").Get<OIDCSettings>() ?? new OIDCSettings();

            using (var scope = services.BuildServiceProvider().CreateScope())
            {

                // ✅ تنظیمات پیشرفته Identity
                services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // 🔐 Password Rules
                    //options.Password.RequiredLength = securitySetting.RequiredLength;
                    //options.Password.RequireDigit = securitySetting.RequireDigit;
                    //options.Password.RequireLowercase = securitySetting.RequireLowercase;
                    //options.Password.RequireUppercase = securitySetting.RequireUppercase;
                    //options.Password.RequireNonAlphanumeric = securitySetting.RequireNonAlphanumeric;

                    // 👤 User Rules
                    options.User.RequireUniqueEmail = true;

                    // 🔒 Lockout
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.AllowedForNewUsers = true;

                    // 📧 Email confirmation
                    options.SignIn.RequireConfirmedEmail = false;



                })
                .AddEntityFrameworkStores<TenantDbContext>()
                .AddDefaultTokenProviders();

                services.AddTransient<IPasswordValidator<ApplicationUser>, DynamicPasswordValidator>();


                // ✅ IdentityServer تنظیمات
                services
                        .AddIdentityServer(options =>
                        {
                            // این خط باعث می‌شود از UI پیش‌فرض Duende استفاده شود
                            // اگر این خط را نگذارید، ممکن است خطای 404 بدهد
                            //options.UserInteraction.LoginUrl = "/identity/account/login";
                            // options.UserInteraction.LogoutUrl = "/identity/account/logout";

                            //// این خط بسیار مهم است: مسیر صفحه لاگین را مشخص می‌کند
                            //options.UserInteraction.LoginUrl = "/Account/Login";

                            //// اگر صفحه خروج هم دارید:
                            //options.UserInteraction.LogoutUrl = "/Account/Logout";

                            //// اگر صفحه خطا هم دارید:
                            //options.UserInteraction.ErrorUrl = "/home/error";


                            options.EmitStaticAudienceClaim = true;
                            //TODO: only development
                            options.Events.RaiseErrorEvents = true;
                            options.Events.RaiseFailureEvents = true;
                            options.Events.RaiseInformationEvents = true;
                            options.Events.RaiseSuccessEvents = true;
                        })
                        .AddAspNetIdentity<ApplicationUser>()
                        .AddInMemoryClients(
                            IdentityServerConfig.Clients)
                        .AddInMemoryApiScopes(
                            IdentityServerConfig.ApiScopes)
                        .AddInMemoryApiResources(
                            IdentityServerConfig.ApiResources)
                        .AddInMemoryIdentityResources(
                            IdentityServerConfig.IdentityResources)
                        .AddProfileService<ProfileService>()
                // TODO: ony use in developement AddDeveloperSigningCredential
                        .AddDeveloperSigningCredential();
                //.AddSigningCredential(new X509Certificate2(Path.Combine(Environment.CurrentDirectory, "path/to/your/cert.pfx"), "certPassword")); // در Production، باید از certificate استفاده کنی

                services.ConfigureApplicationCookie(options =>
                  {
                      options.Cookie.Name = oidcSettings.Cookie.Name;

                      options.Cookie.HttpOnly = oidcSettings.Cookie.HttpOnly;

                      options.Cookie.SecurePolicy = oidcSettings.Cookie.SecurePolicy;

                      options.Cookie.SameSite = oidcSettings.Cookie.SameSite;

                      options.Cookie.Domain = oidcSettings.Cookie.Domain;

                      options.ExpireTimeSpan = TimeSpan.FromDays(30);

                      options.SlidingExpiration = oidcSettings.Cookie.SlidingExpiration;
                  });

                // ✅ JWT Authentication
                //services.AddAuthentication("Bearer")
                //        .AddJwtBearer("Bearer", options =>
                //        {
                //            options.Authority =
                //                "https://localhost:5001";

                //            options.TokenValidationParameters =
                //                new TokenValidationParameters
                //                {
                //                    ValidateAudience = false
                //                };
                //        });

                services.AddAuthentication(IdentityConstants.ApplicationScheme);


                return services;
            }
        }



        public static IServiceCollection AddIdentityForMigrator(this IServiceCollection services, IConfiguration configuration)
        {

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
                   .AddEntityFrameworkStores<TenantDbContext>()
                   .AddDefaultTokenProviders()
                   .AddSignInManager();
            return services;
        }

    }
}
