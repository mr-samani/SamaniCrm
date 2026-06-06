using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Persistence;

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
        },
         ServiceLifetime.Scoped);

        services.AddScoped<IMasterDbContext>(sp =>
        {
            var factory =
                sp.GetRequiredService<IDbContextFactory<MasterDbContext>>();

            return factory.CreateDbContext();
        });



        // Set Tenant 
        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            var currentTenant = sp.GetRequiredService<ICurrentTenant>();
            var tenantId = currentTenant.TenantId;
            Console.WriteLine($"Tenant in DBContext={tenantId}");
            var defaultConnectionString = config.GetConnectionString("DefaultConnection");

            var connectionString = tenantId == null ? defaultConnectionString : currentTenant.ConnectionString;

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
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        services.AddScoped<ApplicationDbInitializer>();
        return services;
    }



}


