using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Persistence;
using SamaniCrm.Infrastructure.Security;
using SamaniCrm.Infrastructure.Services.TenantService;

namespace SamaniCrm.DbMigrator;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string not found.");

        // ✅ DbContext
        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Scoped);

        services.AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Scoped);
        return services;
    }


    public static IServiceCollection AddCustomService(this IServiceCollection services, IConfiguration configuration)
    {
        // Application Services
        services.AddScoped<ICurrentUserService, DummyCurrentUserService>();
        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<ApplicationDbInitializer>();

        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
        services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();


        services.Configure<EncryptionSettings>(options =>
        {
            options.EncryptionKey = configuration["Encryption:Key"]
                ?? throw new InvalidOperationException("Encryption key not configured");
            options.HashSalt = configuration["Encryption:Salt"] ?? string.Empty;
        });

        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IConnectionStringEncryptor, ConnectionStringEncryptor>();
        services.AddSingleton<ISecureRandomGenerator, SecureRandomGenerator>();


        //var serviceProvider = services.BuildServiceProvider();
        //var logger = serviceProvider.GetService<ILogger<ApplicationDbInitializer>>();
        //services.AddSingleton(typeof(ILogger), logger!);
        services.AddScoped<IAuditLogFactory, AuditLogFactory>();

        services.AddLogging(logging => logging.AddConsole());
        services.AddScoped<ICurrentUserService, DummyCurrentUserService>();


        return services;
    }


    public class DummyCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => null;//"MigrationUser";  
        public Guid? TenantId => null;
        public string? UserName => "MigrationUser";

        public string Lang => "fa-IR";


        public bool IsDelegated => false;

        public Guid? DelegatorId => null;
        public bool IsAuthenticated => false;
        public bool IsHost => TenantId == null;
        public bool IsTenantAdmin => TenantId != null;


    }


}
