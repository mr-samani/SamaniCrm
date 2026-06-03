using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.Services.TenantService;
using System.Collections.Concurrent;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        // set master db context
        services.AddSingleton<ApplicationDbContext>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var centralConnectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(centralConnectionString)
                .Options;

            return new ApplicationDbContext(options, null, null, null);
        });

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var currentTenant = sp.GetRequiredService<ICurrentTenant>();
            var tenantId = currentTenant.TenantId;

            var tenantDatabaseService = sp.GetRequiredService<ITenantDatabaseService>();
            var cache = sp.GetRequiredService<ICacheService>();


            // 3. دریافت کانکشن استرینگ پویا
            var connectionString = GetConnectionStringForTenant(tenantId, tenantDatabaseService, cache, config);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string not found for tenant: {tenantId}");
            }

            // تنظیمات EF
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(30);
            });
        }, ServiceLifetime.Scoped);

        // 4. ثبت اینترفیس به صورت Scoped
        // این باعث می‌شود DI بتواند ApplicationDbContext را تزریق کند
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbInitializer>();
        return services;
    }

    private static string? GetConnectionStringForTenant(Guid? tenantId,
        ITenantDatabaseService tenantDbService,
        ICacheService cache, IConfiguration config)
    {
        if (tenantId.HasValue == false)
        {
            return config.GetConnectionString("DefaultConnection");
        }


        var cacheKey = CacheKeys.TenantConnectionString_ + tenantId.ToString();


        var connectionString = cache.GetAsync<string?>(cacheKey).Result;


        // Check cache first
        if (string.IsNullOrEmpty(connectionString) == false)
        {
            return connectionString;
        }

        var encryptedConn = tenantDbService.GetEncryptedConnectionString(tenantId);

        if (string.IsNullOrEmpty(encryptedConn))
        {
            return config.GetConnectionString("DefaultConnection");
        }

        // 3. رمزگشایی
        connectionString = tenantDbService.DecryptConnectionString(encryptedConn);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"No database connection found for tenant {tenantId}");
        }


        cache.SetAsync(cacheKey, connectionString);

        return connectionString;
    }



}