using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Data.Seeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthStatus = SamaniCrm.Domain.Entities.HealthStatus;

namespace SamaniCrm.Infrastructure.Services.TenantService;

public interface ITenantProvisioningService
{
    Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation);
    Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation);
    Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation);
    Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation);
}


public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITenantDatabaseService _databaseService;
    private readonly ITenantNotificationService _notificationService;
    private readonly ILogger<TenantProvisioningService> _logger;
    private readonly IEnumerable<ITenantDataSeeder> _seeders;

    public TenantProvisioningService(
        ITenantDatabaseService databaseService,
        ITenantNotificationService notificationService,
        ILogger<TenantProvisioningService> logger,
        IEnumerable<ITenantDataSeeder> seeders,
        ApplicationDbContext dbContext)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;
        _logger = logger;
        _seeders = seeders;
        _dbContext = dbContext;
    }

    public async Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation)
    {
        var databaseName = $"Tenant_{tenant.Slug}_{tenant.Id:N}";
        var serverName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var adminUsername = Environment.GetEnvironmentVariable("DB_ADMIN_USER") ?? "sa";
        var adminPassword = Environment.GetEnvironmentVariable("DB_ADMIN_PASSWORD") ?? "";

        // Create database
        await _databaseService.CreateDatabaseAsync(
            serverName, databaseName, adminUsername, adminPassword, cancellation);

        // Generate connection string
        var connectionString = _databaseService.GenerateConnectionString(
            serverName, databaseName, adminUsername, adminPassword);

        // Save encrypted connection string
        tenant.DatabaseName = databaseName;
        tenant.ServerName = serverName;
        tenant.ConnectionString = _databaseService.EncryptConnectionString(connectionString);

        // Save database connection record
        var dbConnection = new TenantDatabaseConnection
        {
            TenantId = tenant.Id,
            DatabaseType = DatabaseType.SQLServer,
            ServerName = serverName,
            DatabaseName = databaseName,
            Username = adminUsername,
            EncryptedPassword = _databaseService.Encrypt(adminPassword),
            ConnectionString = tenant.ConnectionString,
            IsMaster = false,
            IsActive = true,
            HealthStatus = HealthStatus.Healthy,
            
        };

        await _dbContext.TenantDatabaseConnections.AddAsync(dbConnection, cancellation);
        await _dbContext.SaveChangesAsync(cancellation);

        _logger.LogInformation("Database {DatabaseName} created for tenant {TenantId}",
            databaseName, tenant.Id);
    }

    public async Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation)
    {
        if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated && !string.IsNullOrEmpty(tenant.ConnectionString))
        {
            var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
            await _databaseService.RunMigrationsAsync(connectionString,tenant.Id, cancellation);
        }
        else
        {
            // For shared database, migrations are handled by EF Core automatically
            await _dbContext.Database.MigrateAsync(cancellation);
        }
    }

    public async Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.SeedAsync(tenant, cancellation);
        }
    }

    public async Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        return await _databaseService.TestConnectionAsync(connectionString, cancellation);
    }
}
