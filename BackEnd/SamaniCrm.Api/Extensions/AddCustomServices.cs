using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Api.Middlewares;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.NotificationManager.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using SamaniCrm.Application.SubscriptionManager.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.AuditLog;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Captcha;
using SamaniCrm.Infrastructure.Connections;
using SamaniCrm.Infrastructure.Data;
using SamaniCrm.Infrastructure.Email;
using SamaniCrm.Infrastructure.ExternalLogin;
using SamaniCrm.Infrastructure.FileManager;
using SamaniCrm.Infrastructure.Hubs;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Jobs;
using SamaniCrm.Infrastructure.Localizer;
using SamaniCrm.Infrastructure.Repositories;
using SamaniCrm.Infrastructure.Security;
using SamaniCrm.Infrastructure.Services;
using SamaniCrm.Infrastructure.Services.Product;
using SamaniCrm.Infrastructure.Services.TenantService;
using SamaniCrm.Infrastructure.Storage;
using SamaniCrm.Infrastructure.SubscriptionManager;

namespace SamaniCrm.Api.Extensions;


public static partial class ServiceCollectionExtensions
{

    /// <summary>
    /// ثبت سرویس های برنامه
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<LocalizationMemoryCache>();
        services.AddSingleton<ISecretStore, ConfigurationSecretStore>();
        services.AddSingleton<IConnectionManager, SignalRConnectionManager>();
        services.AddScoped<INotificationSender, NotificationSender>();



        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IEmailSender<ApplicationUser>, MyEmailSender>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<IExternalLoginService, ExternalLoginService>();


        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<ILocalizer, CachedStringLocalizer>();



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

        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<FileDirectoryInitializer>();


        // Multi-Tenancy
        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<ITenantResolver, TenantResolver>();
       // services.AddSingleton<IUserIdProvider, TenantUserIdProvider>();
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




        services.AddScoped<ISubscriptionService, SubscriptionService>();




        return services;
    }

}