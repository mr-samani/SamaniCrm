using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.Services.TenantService;
using System.Collections.Concurrent;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        // set master db context 
        services.AddDbContextFactory<MasterDbContext>(options =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Master Connection string not found!");
            }
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(30);
            });
        });
        services.AddScoped<IMasterDbContext>(sp => sp.GetRequiredService<MasterDbContext>());




        services.AddDbContext<TenantDbContext>((sp, options) =>
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
        // services.AddScoped<IApplicationDbContext, TenantDbContext>();
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<TenantDbContext>());

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