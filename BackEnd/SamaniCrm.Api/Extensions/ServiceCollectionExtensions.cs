using FluentValidation;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Infrastructure.AuditLog;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.Data;
using SamaniCrm.Infrastructure.Email;
using SamaniCrm.Infrastructure.ExternalLogin;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Hubs;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Jobs;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Loging;
using SamaniCrm.Infrastructure.Loging.Sinks;
using SamaniCrm.Infrastructure.MappingProfile;
using SamaniCrm.Infrastructure.Repositories;
using SamaniCrm.Infrastructure.Security;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Infrastructure.Services.Product;
using SamaniCrm.Infrastructure.Services.TenantService;
using SamaniCrm.Infrastructure.Storage;
using Scalar.AspNetCore;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using SamaniCrm.Infrastructure.Loging.Filters;
using SamaniCrm.Infrastructure.Loging.Decorators;
using SamaniCrm.Infrastructure;



namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        // ✅ DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(30);
            }),
            ServiceLifetime.Scoped);


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


            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // TODO
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantValidationBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantAuthorizationBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));


        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }


    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { },
        typeof(ExternalProviderProfile)
          //typeof(ProfileTypeFromAssembly1), 
          //typeof(ProfileTypeFromAssembly2) 
          );
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

                // opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            });

        return services;
    }


    /// <summary>
    /// Swagger Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", opt =>
        {
            opt.AddSchemaTransformer<SchemaTransformer>();
            opt.AddOperationTransformer<OperationSchmaTransformer>();

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
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
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

        // Jobs
        services.AddScoped<ILoginJobsService, LoginJobsService>();
        services.AddScoped<ICreateTenantJobService, CreateTenantJobService>();

        //چون حافظه ایه Singleton باشه بهتره.
        services.AddSingleton<ICaptchaStore, InMemoryCaptchaStore>();
        services.AddHostedService<CaptchaCleanupBackgroundService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();


        services.AddScoped<PermissionFilterMiddleware>();
        services.AddControllers(options =>
        {
            options.Filters.Add<PermissionFilterMiddleware>();
        });

        services.AddScoped<INotificationHubService, NotificationHubService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<FileDirectoryInitializer>();


        // Multi-Tenancy
        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddSingleton<IUserIdProvider, TenantUserIdProvider>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantService, TenantService>();



        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();
        services.AddScoped<ITenantUniquenessChecker, TenantUniquenessChecker>();
        services.AddScoped<ITenantNotificationService, TenantNotificationService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IAuditLogService, AuditLogService>();



        // Encryption
        services.Configure<EncryptionSettings>(options =>
        {
            options.EncryptionKey = config["Encryption:Key"]
                ?? throw new InvalidOperationException("Encryption key not configured");
            options.HashSalt = config["Encryption:Salt"] ?? string.Empty;
        });

        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IConnectionStringEncryptor, ConnectionStringEncryptor>();
        services.AddSingleton<ISecureRandomGenerator, SecureRandomGenerator>();

        // Data Protection
        services.AddDataProtection()
            .SetApplicationName("MultiTenantApp")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        services.AddSingleton<ITenantDataProtector, TenantDataProtector>();

        // Tenant DbContext Factory
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();


        // Seeders
        // TODO
        //services.AddScoped<ITenantDataSeeder, CategorySeeder>();
        //services.AddScoped<ITenantDataSeeder, SettingsSeeder>();
        //services.AddScoped<ITenantDataSeeder, WorkflowSeeder>();





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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("TenantOwner", policy =>
                policy.RequireClaim("tenant_role", "Owner"));
            options.AddPolicy("TenantAdmin", policy =>
                policy.RequireClaim("tenant_role", "Owner", "Admin"));
        });

        return services;
    }




    public static IServiceCollection AddHangfireJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IStartupFilter, HangfireJobStartupFilter>();
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
                        services.AddAuthentication().AddOAuth(provider.Scheme!, options =>
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
                        //case ExternalProviderTypeEnum.OpenIdConnect:
                        //    services.AddAuthentication().AddOpenIdConnect(provider.Scheme!, options =>
                        //    {
                        //        options.Authority = provider.MetadataJson; // or metadata URL
                        //        options.ClientId = provider.ClientId ?? secretStore.GetSecret("OpenIdConnect.ClientId");
                        //        options.ClientSecret = secretStore.GetSecret("OpenIdConnect.ClientSecret");
                        //        options.CallbackPath = provider.CallbackPath != "" ? provider.CallbackPath : $"/signin-{provider.Scheme}";
                        //        options.SignInScheme = IdentityConstants.ExternalScheme;
                        //        options.ResponseType = "code";
                        //        // ...map scopes/claims
                        //    });
                        //    break;
                        // ... برای LinkedIn, Twitter, ...
                }


            }
        }
        return services;
    }




    public static IServiceCollection AddHelthChecks(this IServiceCollection services, IConfiguration config)
    {
        // TODO
        //services.AddHealthChecks()
        // .AddDbContextCheck<ApplicationDbContext>("master_db");
        //  .AddCheck<TenantDatabaseHealthCheck>("tenant_db");
        return services;
    }


    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration config)
    {
        // Sink ها
        services.AddSingleton<FileLogSink>();
        services.AddScoped<DatabaseLogSink>();
        services.AddSingleton<TelegramLogSink>();
        services.AddSingleton<BaleLogSink>();
        services.AddSingleton<ExternalApiLogSink>();

        // ثبت Sink ها به صورت IEnumerable
        services.AddScoped<IEnumerable<ILogSink>>(sp => new ILogSink[]
        {
            sp.GetRequiredService<FileLogSink>(),
            sp.GetRequiredService<DatabaseLogSink>(),
            sp.GetRequiredService<TelegramLogSink>(),
            sp.GetRequiredService<BaleLogSink>(),
            sp.GetRequiredService<ExternalApiLogSink>()
        });

        // Core Services
        services.AddScoped<ILogConfigurationService, LogConfigurationService>();
        services.AddScoped<ILogService, LogService>();
        // services.AddScoped<ILogRetentionService, LogRetentionService>();


        // Background Service
        services.AddHostedService<LogRetentionService>();

        // Action Filter
        services.AddLoggingInterceptors();

        // Decorator برای Service ها
        services.AddLoggedServices();

        return services;
    }

}




