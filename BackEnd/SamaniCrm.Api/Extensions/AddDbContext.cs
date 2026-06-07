using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.TenantManager;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(config);

        services.AddSingleton<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<ICurrentTenant, CurrentTenant>();

        services.AddDbContextFactory<MasterDbContext>(options =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Master connection string not found.");

            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(30);
            });
        }, ServiceLifetime.Scoped);

        services.AddScoped<IMasterDbContext>(sp =>
        {
            var factory = sp.GetRequiredService<IDbContextFactory<MasterDbContext>>();
            return factory.CreateDbContext();
        });

     

        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<TenantConnectionInterceptor>();

            var defaultConnectionString = config.GetConnectionString("DefaultConnection");

            options.UseSqlServer(defaultConnectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
            })
            .AddInterceptors(interceptor); 
        }, ServiceLifetime.Scoped);


        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<TenantDbContext>());

        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        services.AddScoped<ApplicationDbInitializer>();

        return services;
    }

}


