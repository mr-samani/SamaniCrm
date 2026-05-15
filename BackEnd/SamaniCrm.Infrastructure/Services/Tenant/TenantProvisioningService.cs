using Azure.Core;
using Hangfire;
using Hangfire.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Data.Seeder;
using SamaniCrm.Infrastructure.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthStatus = SamaniCrm.Domain.Entities.HealthStatus;

namespace SamaniCrm.Infrastructure.Services.TenantService;






public interface ITenantProvisioningService
{
    Task InitializeTenantProvisionSteps(Guid tenantId, CancellationToken cancellation);
    Task<List<TenantProvisionStepsEnum>> GetPendingProvisionSteps(Guid tenantId, CancellationToken cancellation);
    Task ProvisionCreateAdminUser(TenantJobProvisioningData request, Guid tenantId, CancellationToken cancellation);
    Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation);
    Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation);
    Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation);
    Task FinalizeAsync(Tenant tenant, CancellationToken cancellation);

    Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation);
}


public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITenantDatabaseService _databaseService;
    private readonly ITenantNotificationService _notificationService;
    private readonly ILogger<TenantProvisioningService> _logger;
    private readonly IEnumerable<ITenantDataSeeder> _seeders;
    private readonly IIdentityService _identityService;

    public TenantProvisioningService(
        ITenantDatabaseService databaseService,
        ITenantNotificationService notificationService,
        ILogger<TenantProvisioningService> logger,
        IEnumerable<ITenantDataSeeder> seeders,
        ApplicationDbContext dbContext,
        IIdentityService identityService)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;
        _logger = logger;
        _seeders = seeders;
        _dbContext = dbContext;
        _identityService = identityService;
    }




    public async Task InitializeTenantProvisionSteps(Guid tenantId, CancellationToken cancellation)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .Include(p => p.ProvisioningSteps)
            .Where(x => x.Id == tenantId)
            .FirstOrDefaultAsync(cancellation);

        if (tenant == null)
        {
            throw new NotFoundException("Tenant not found!");
        }

        List<TenantProvisioningStep> allProvisioningSteps = new()
            {
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.CreateTenant.ToString(), Description = "ایجاد موجودیت بهره‌بردار", Status = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.CreateAdminUser.ToString(), Description = "ایجاد کاربر مدیر", Status = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.ProvisionDatabase.ToString(), Description = "ایجاد دیتابیس", Status = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.RunMigrations.ToString(), Description = "اجرای مایگریشن‌ها", Status = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.SeedData.ToString(), Description = "بارگذاری داده‌های اولیه", Status = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Name = TenantProvisionStepsEnum.Finalize.ToString(), Description = "تکمیل تنظیمات", Status = ProvisioningStepStatus.Pending }
            };
        List<string> existSteps = tenant.ProvisioningSteps?.Select(s => s.Name).ToList() ?? new();
        var notExistSteps = allProvisioningSteps.Where(x => existSteps.Contains(x.Name) == false).ToList();

        if (notExistSteps.Any())
        {
            await _dbContext.TenantProvisioningSteps.AddRangeAsync(notExistSteps);
            await _dbContext.SaveChangesAsync(cancellation);
        }

    }

    public async Task<List<TenantProvisionStepsEnum>> GetPendingProvisionSteps(Guid tenantId, CancellationToken cancellation)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .Include(p => p.ProvisioningSteps)
            .Where(x => x.Id == tenantId)
            .FirstOrDefaultAsync(cancellation);
        if (tenant == null)
        {
            throw new NotFoundException("Tenant not found!");
        }

        return tenant.ProvisioningSteps?
            .Select(s => Enum.TryParse(s.Name, out TenantProvisionStepsEnum result) ? result : default!)
            .Where(w => w != default)
            .ToList() ?? [];
    }





    public async Task ProvisionCreateAdminUser(TenantJobProvisioningData request, Guid tenantId, CancellationToken cancellation)
    {
        await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress,
            request.Slug,
            tenantId,
            "Creating admin user...",
            TenantProvisionStepsEnum.CreateAdminUser);

        var adminUser = new CreateUserCommand()
        {
            Email = request.AdminEmail.ToLowerInvariant(),
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Password = request.AdminPassword,
            UserName = request.AdminUserName,
            PhoneNumber = request.AdminMobile,
            Address = request.Address,
            Lang = AppConsts.DefaultLanguage,
            Roles = [Roles.TenantAdministrator],
        };
        var createUserResult = await _identityService.CreateUserAsync(adminUser);
        if (createUserResult.isSucceed == false)
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed,
             request.Slug,
            tenantId,
             "Can not create tenant admin user.",
             TenantProvisionStepsEnum.CreateAdminUser);
        }
        else
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
           request.Slug,
            tenantId,
           "Created admin user",
           TenantProvisionStepsEnum.CreateAdminUser);
        }
    }




    public async Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation)
    {
        if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated)
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
                 tenant.Slug,
          tenant.Id,
                 "Check Database Ok",
                 TenantProvisionStepsEnum.ProvisionDatabase);
            return;
        }
        await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress,
        tenant.Slug,
        tenant.Id,
        "Creating isolated database...",
        TenantProvisionStepsEnum.ProvisionDatabase);
        var databaseName = $"Tenant_{tenant.Slug}_{tenant.Id:N}";
        var serverName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var adminUsername = Environment.GetEnvironmentVariable("DB_ADMIN_USER") ?? "sa";
        var adminPassword = Environment.GetEnvironmentVariable("DB_ADMIN_PASSWORD") ?? "";


        try
        {

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
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
                   tenant.Slug,
            tenant.Id,
                   "Created isolated database",
                   TenantProvisionStepsEnum.ProvisionDatabase);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error on Create Database {DatabaseName} for tenant {TenantId}", databaseName, tenant.Id);
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed,
                       tenant.Slug,
            tenant.Id,
                      ex.Message,
                       TenantProvisionStepsEnum.ProvisionDatabase);
        }
    }

    public async Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation)
    {
        await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress,
                 tenant.Slug,
            tenant.Id,
                 "Running database migrations...",
                 TenantProvisionStepsEnum.RunMigrations);
        try
        {
            if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated && !string.IsNullOrEmpty(tenant.ConnectionString))
            {
                var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
                await _databaseService.RunMigrationsAsync(connectionString, tenant.Id, cancellation);
            }
            else
            {
                // For shared database, migrations are handled by EF Core automatically
                await _dbContext.Database.MigrateAsync(cancellation);
            }
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
                tenant.Slug,
            tenant.Id,
                "Complete Runn database migrations",
                TenantProvisionStepsEnum.RunMigrations);
        }
        catch (Exception ex)
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed,
               tenant.Slug,
            tenant.Id,
              ex.Message,
               TenantProvisionStepsEnum.RunMigrations);
        }


    }

    public async Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation)
    {
        await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress,
              tenant.Slug,
            tenant.Id,
             "Seeding initial data...",
              TenantProvisionStepsEnum.SeedData);

        try
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync(tenant, cancellation);
            }
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
              tenant.Slug,
            tenant.Id,
             "Complete seed database",
              TenantProvisionStepsEnum.SeedData);
        }
        catch (Exception ex)
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed,
               tenant.Slug,
            tenant.Id,
              ex.Message,
               TenantProvisionStepsEnum.SeedData);
        }
    }


    public async Task FinalizeAsync(Tenant tenant, CancellationToken cancellation)
    {
        await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress,
              tenant.Slug,
            tenant.Id,
             "Finalizing create tenant ...",
              TenantProvisionStepsEnum.Finalize);

        try
        {
            var tenantEntity = await _dbContext.Tenants
                     .Where(x => x.Id == tenant.Id)
                 .FirstOrDefaultAsync(cancellation);
            if (tenantEntity != null)
            {

                tenantEntity.ProvisioningStatus = ProvisioningStatus.Ready;
                tenantEntity.Status = TenantStatus.Active;
                tenantEntity.ModifiedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync(cancellation);
            }
            _logger.LogInformation(
                "Tenant {TenantId} ({Slug}) created successfully", tenant.Id, tenant.Slug);

            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed,
              tenant.Slug,
            tenant.Id,
             "Create tenant and activation successfully",
              TenantProvisionStepsEnum.SeedData);
        }
        catch (Exception ex)
        {
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed,
               tenant.Slug,
            tenant.Id,
              ex.Message,
               TenantProvisionStepsEnum.Finalize);
        }
    }




    public async Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        return await _databaseService.TestConnectionAsync(connectionString, cancellation);
    }






    private async Task SendNotificationProcessAsync(
        ProvisioningNotificationStatus status, string tenantSlug, Guid tenantId, string message, TenantProvisionStepsEnum step)
    {
        var stepEntity = await _dbContext.TenantProvisioningSteps
               .Where(x => x.TenantId == tenantId && x.Name == step.ToString())
               .FirstOrDefaultAsync();
        switch (status)
        {
            case ProvisioningStepStatus.InProgress:
                await _notificationService.SendProgressAsync(tenantSlug, message, step);
                if (stepEntity != null)
                {
                    stepEntity.Status = ProvisioningStepStatus.InProgress;
                    stepEntity.StartedAt = DateTime.UtcNow;
                    stepEntity.CompletedAt = null;
                    await _dbContext.SaveChangesAsync();
                }
                break;
            case ProvisioningStepStatus.Failed:
                await _notificationService.SendErrorAsync(tenantSlug, message, step);
                if (stepEntity != null)
                {
                    stepEntity.Status = ProvisioningStepStatus.Failed;
                    stepEntity.CompletedAt = null;
                    await _dbContext.SaveChangesAsync();
                }
                break;
            case ProvisioningStepStatus.Completed:
                await _notificationService.SendCompletionAsync(tenantSlug, message, step);
                if (stepEntity != null)
                {
                    stepEntity.Status = ProvisioningStepStatus.Completed;
                    stepEntity.CompletedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
                break;

        }





    }

}
