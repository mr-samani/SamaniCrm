using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Infrastructure.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {  
            
 

            using (var scope = services.BuildServiceProvider().CreateScope())
            {

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var securitySetting = dbContext.SecuritySettings.FirstOrDefault(); // یا هر روش دیگه‌ای برای خوندن

                if (securitySetting == null)
                    throw new Exception("Security settings not found in the database.");



                // ✅ تنظیمات پیشرفته Identity
                services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // 🔐 Password Rules
                    options.Password.RequiredLength = securitySetting.RequiredLength;
                    options.Password.RequireDigit = securitySetting.RequireDigit;
                    options.Password.RequireLowercase = securitySetting.RequireLowercase;
                    options.Password.RequireUppercase = securitySetting.RequireUppercase;
                    options.Password.RequireNonAlphanumeric = securitySetting.RequireNonAlphanumeric;

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

                services.AddTransient<IPasswordValidator<ApplicationUser>, DynamicPasswordValidator>();


                // ✅ IdentityServer تنظیمات
                services.AddIdentityServer(options =>
                {
                    options.EmitStaticAudienceClaim = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();
                //.AddSigningCredential(new X509Certificate2(Path.Combine(Environment.CurrentDirectory, "path/to/your/cert.pfx"), "certPassword")); // در Production، باید از certificate استفاده کنی

                // ✅ JWT Authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                // بررسی اینکه آیا توکن معتبر است یا نه
                                var request = context.Request;
                                if (request.Headers.ContainsKey("Authorization"))
                                {
                                    var token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                                    // بررسی توکن اشتباه یا منقضی
                                    if (string.IsNullOrEmpty(token))
                                    {
                                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                        context.Response.ContentType = "application/json";
                                        var result = JsonSerializer.Serialize(new { error = "Unauthorized - Invalid Token" });
                                        await context.Response.WriteAsync(result);
                                    }
                                }
                                else
                                {
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    context.Response.ContentType = "application/json";
                                    var result = JsonSerializer.Serialize(new { error = "Unauthorized - No Token" });
                                    await context.Response.WriteAsync(result);
                                }
                            }
                        };
                    });

                // ✅ اضافه کردن Identity API Endpoints برای minimal APIs در .NET 7+
                // services.AddIdentityApiEndpoints<ApplicationUser>();

                return services;
            }
        }
    }
}
