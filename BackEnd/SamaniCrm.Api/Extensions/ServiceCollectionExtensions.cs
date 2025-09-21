using FluentValidation;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.Email;
using SamaniCrm.Infrastructure.ExternalLogin;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Jobs;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Notifications;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Infrastructure.Services.Product;
using SamaniCrm.Infrastructure.Storage;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace SamaniCrm.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        // ✅ DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")),
            ServiceLifetime.Transient);
        return services;
    }


    /// <summary>
    /// Mediator Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // cfg.RegisterServicesFromAssembly(typeof(UserListQuery).Assembly);
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(GetRoleQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(InitialAppQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetCurrentUserQueryHandler).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }


    /// <summary>
    /// فعال سازی ولیدیشن های ورودی به برنامه با استفاده از FluentValidation
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<GetUserQueryValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        return services;
    }



    /// <summary>
    /// راه اندازی زیرساخت های برنامه
    /// </summary>
    /// <remarks>
    ///  DBContext,
    ///  IDentity,
    ///  Etc...
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityInfrastructure(config);
        return services;
    }


    /// <summary>
    /// اجازه دسترسی به برنامه توسط کلاینت ها
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
            {
                policy.WithOrigins("http://localhost:5753", "https://localhost:5753", "https://localhost:5754")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithExposedHeaders("Location", "Upload-Offset", "Tus-Resumable", "Upload-Length", "Fileid");
            });
        });

        return services;
    }

    public static IServiceCollection AddControllersWithDefaults(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                //options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                //options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status401Unauthorized));
                // options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status403Forbidden));
            })
            .AddJsonOptions(opt =>
            {
                // اگر این نباشه کاراکتر های فارسی یونیکد میشن 
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                // تبدیل خروجی به camelCase 
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                // فیلد های نال توی خروجی نمیان
                // opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //از کرش شدن به خاطر حلقه های مرجع (reference loops) جلوگیری می کنه.
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // برای اینکه فیلد های خالی را هم در خروجی بیاره
                opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
            });

        return services;
    }


    /// <summary>
    /// Swagger Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SamaniCrm API", Version = "v1" });
            c.AddServer(new OpenApiServer
            {
                Url = "https://localhost:44343",
                Description = "localhost"
            });
            c.AddServer(new OpenApiServer
            {
                Url = "https://api.samani-crm.com",
                Description = "Production Server"
            });
            c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.SchemaFilter<AddEnumNamesSchemaFilter>();

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }


    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseColouredConsoleLogProvider()
                .UseConsole()
                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }

    /// <summary>
    /// ثبت سرویس های برنامه
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
        services.AddTransient<IEmailSender<ApplicationUser>, MyEmailSender>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<IExternalLoginService, ExternalLoginService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ILanguageService, LanguageService>();
        services.AddSingleton<LocalizationMemoryCache>();
        services.AddScoped<ILocalizer, CachedStringLocalizer>();
        services.AddSingleton<ISecretStore, ConfigurationSecretStore>();



        services.AddScoped<ISecuritySettingService, SecuritySettingService>();
        services.AddScoped<ILoginJobsService, LoginJobsService>();


        //چون حافظه ایه Singleton باشه بهتره.
        services.AddSingleton<ICaptchaStore, InMemoryCaptchaStore>();
        services.AddHostedService<CaptchaCleanupBackgroundService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();


        services.AddScoped<PermissionFilter>();
        services.AddControllers(options =>
        {
            options.Filters.Add<PermissionFilter>();
        });

        services.AddScoped<INotificationHubService, NotificationHubService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<FileDirectoryInitializer>();



        return services;
    }




    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        //services.AddAuthorization(options =>
        //{
        //    foreach (var permission in Enum.GetNames(typeof(Permission)))
        //    {
        //        options.AddPolicy($"Permission:{permission}", policy =>
        //        {
        //            policy.Requirements.Add(new PermissionRequirement(permission));
        //        });
        //    }
        //});

        //services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }




    public static IServiceCollection AddHangfireJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IStartupFilter, HangfireJobStartupFilter>();
        return services;
    }
    public static IServiceCollection LoadExternalProviders(this IServiceCollection services, IConfiguration config)
    {
        // load external provider configs (sync for bootstrap; or async with factory)
        using (var sp = services.BuildServiceProvider())
        {
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var secretStore = sp.GetRequiredService<ISecretStore>(); // wrapper for KeyVault / DPAPI
            var providers = db.ExternalProviders.Where(p => p.IsActive).ToList();

            foreach (var provider in providers)
            {
                switch (provider.ProviderType)
                {
                    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-9.0
                    case ExternalProviderTypeEnum.Google:
                        services.AddAuthentication().AddGoogle(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Google:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Google:ClientSecret");
                            options.CallbackPath = provider.CallbackPath ?? $"/signin-{provider.Scheme}";
                            options.SignInScheme = IdentityConstants.ExternalScheme;
                            // map claims if needed
                        });
                        break;
                    // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-10.0
                    case ExternalProviderTypeEnum.Microsoft:
                        services.AddAuthentication().AddMicrosoftAccount(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Microsoft:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Microsoft:ClientSecret");
                        });
                        break;

                    case ExternalProviderTypeEnum.Facebook:
                        services.AddAuthentication().AddFacebook(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Facebook:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Facebook:ClientSecret");
                        });
                        break;

                    case ExternalProviderTypeEnum.GitHub:
                        services.AddAuthentication().AddGitHub(options =>
                        {
                            options.ClientId = secretStore.GetSecret("GitHub:ClientId");
                            options.ClientSecret = secretStore.GetSecret("GitHub:ClientSecret");
                        });
                        break;
                    //https://learn.microsoft.com/en-us/linkedin/shared/authentication/client-credentials-flow?tabs=HTTPS1
                    case ExternalProviderTypeEnum.LinkedIn:
                        services.AddAuthentication().AddLinkedIn(options =>
                         {
                             options.ClientId = secretStore.GetSecret("LinkedIn:ClientId");
                             options.ClientSecret = secretStore.GetSecret("LinkedIn:ClientSecret");
                         });
                        break;
                    case ExternalProviderTypeEnum.Twitter:
                        services.AddAuthentication().AddTwitter(options =>
                         {
                             options.ConsumerKey = secretStore.GetSecret("Twitter:ClientId");
                             options.ConsumerSecret = secretStore.GetSecret("Twitter:ClientSecret");
                         });
                        break;
                    case ExternalProviderTypeEnum.OAuth2:
                        services.AddAuthentication().AddOAuth(provider.Scheme, options =>
                        {
                            options.ClientId = secretStore.GetSecret("OAuth2.ClientId") ?? "myOath";
                            options.ClientSecret = secretStore.GetSecret("OAuth2.ClientSecret") ?? "myOath";
                            options.AuthorizationEndpoint = provider.AuthorizationEndpoint;
                            options.TokenEndpoint = provider.TokenEndpoint;
                            options.UserInformationEndpoint = provider.UserInfoEndpoint;
                            options.CallbackPath = provider.CallbackPath ?? $"/signin-{provider.Scheme}";
                            options.SignInScheme = IdentityConstants.ExternalScheme;

                            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

                            options.Events = new OAuthEvents
                            {
                                OnCreatingTicket = async ctx =>
                                {
                                    var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                                    var resp = await ctx.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ctx.HttpContext.RequestAborted);
                                    resp.EnsureSuccessStatusCode();
                                    var user = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                                    ctx.RunClaimActions(user.RootElement);
                                }
                            };
                        });
                        break;
                    case ExternalProviderTypeEnum.OpenIdConnect:
                        services.AddAuthentication().AddOpenIdConnect(provider.Scheme, options =>
                        {
                            options.Authority = provider.MetadataJson; // or metadata URL
                            options.ClientId = secretStore.GetSecret("OpenIdConnect.ClientId");
                            options.ClientSecret = secretStore.GetSecret("OpenIdConnect.ClientSecret");
                            options.CallbackPath = provider.CallbackPath ?? $"/signin-{provider.Scheme}";
                            options.SignInScheme = IdentityConstants.ExternalScheme;
                            options.ResponseType = "code";
                            // ...map scopes/claims
                        });
                        break;
                        // ... برای LinkedIn, Twitter, ...
                }


            }
        }
        return services;
    }
    public class HangfireJobStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                jobManager.AddOrUpdate<ILoginJobsService>(
                    recurringJobId: "ReleaseExpiredLocksJob",
                    methodCall: job => job.ReleaseExpiredLocksAsync(null!),
                    cronExpression: "*/30 * * * * *",
                     options: new RecurringJobOptions()
                     {
                         MisfireHandling = MisfireHandlingMode.Relaxed,
                         TimeZone = TimeZoneInfo.Utc,
                     }
                );

                next(app);
            };
        }
    }

}


public class AddEnumNamesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsEnum)
        {
            var enumNames = Enum.GetNames(type);
            var enumNamesArray = new OpenApiArray();
            foreach (var name in enumNames)
            {
                enumNamesArray.Add(new OpenApiString(name));
            }

            schema.Extensions.Add("x-enum-varnames", enumNamesArray);
        }
    }
}




