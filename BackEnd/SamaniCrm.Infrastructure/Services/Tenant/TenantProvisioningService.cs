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
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Data.Seeder;
using SamaniCrm.Infrastructure.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthStatus = SamaniCrm.Domain.Entities.HealthStatus;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Consts;


namespace SamaniCrm.Infrastructure.Services.TenantService;


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
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.CreateTenant, Description = "ایجاد موجودیت بهره‌بردار", StepStatus = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.CreateAdminUser, Description = "ایجاد کاربر مدیر", StepStatus = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.ProvisionDatabase, Description = "ایجاد دیتابیس", StepStatus = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.RunMigrations, Description = "اجرای مایگریشن‌ها", StepStatus = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.SeedData, Description = "بارگذاری داده‌های اولیه", StepStatus = ProvisioningStepStatus.Pending },
                new() {TenantId = tenantId, Step = TenantProvisionStepsEnum.Finalize, Description = "تکمیل تنظیمات", StepStatus = ProvisioningStepStatus.Pending }
            };
        List<TenantProvisionStepsEnum> existSteps = tenant.ProvisioningSteps?.Select(s => s.Step).ToList() ?? new();
        var notExistSteps = allProvisioningSteps.Where(x => existSteps.Contains(x.Step) == false).ToList();

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
            .Select(s => s.Step)
            .Where(w => w != default)
            .ToList() ?? [];
    }





    public async Task ProvisionCreateAdminUser(TenantJobProvisioningData request, Guid tenantId, CancellationToken cancellation)
    {
        var admin = await _identityService.GetTenantAdmin(tenantId, cancellation);
        if (admin != null && request.AdminUserName == admin.UserName)
        {
            return;
        }

        var adminUser = new CreateUserCommand()
        {
            TenantId = tenantId,
            Email = request.AdminEmail.ToLowerInvariant(),
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Password = request.AdminPassword,
            UserName = request.AdminUserName,
            PhoneNumber = request.AdminMobile,
            Address = request.Address,
            Lang = AppConsts.DefaultLanguage,
            Roles = [AppRoles.TenantAdministrator],
        };
        var createUserResult = await _identityService.CreateUserAsync(adminUser);
        if (createUserResult.isSucceed == false)
        {
            throw new Exception("Can not create Admin User!");
        }
    }




    public async Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation)
    {
        var databaseName = $"Tenant_{tenant.Slug}_{tenant.Id:N}";
        var serverName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var adminUsername = Environment.GetEnvironmentVariable("DB_ADMIN_USER") ?? "sa";
        var adminPassword = Environment.GetEnvironmentVariable("DB_ADMIN_PASSWORD") ?? "123456";


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
            await _databaseService.RunMigrationsAsync(connectionString, tenant.Id, cancellation);
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


    public async Task FinalizeAsync(Tenant tenant, CancellationToken cancellation)
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

    }




    public async Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        return await _databaseService.TestConnectionAsync(connectionString, cancellation);
    }






    public async Task<List<ProvisioningStatusDto>> GetTenantProvisionSteps(Guid tenantId, CancellationToken cancellation, bool? ChackInit = true)
    {
        var result = await _dbContext.TenantProvisioningSteps
            .AsNoTracking()
            .Select(s => new ProvisioningStatusDto()
            {
                TenantId = s.TenantId,
                Step = s.Step,
                StepStatus = s.StepStatus,
                StartedAt = s.StartedAt,
                CompletedAt = s.CompletedAt,
                ErrorMessage = s.ErrorMessage,
                RetryCount = s.RetryCount,
            })
               .Where(x => x.TenantId == tenantId)
               .OrderBy(x => x.Step)
               .ToListAsync(cancellation);
        if (result.Count == 0 && ChackInit == true)
        {
            await InitializeTenantProvisionSteps(tenantId, cancellation);
            return await GetTenantProvisionSteps(tenantId, cancellation, false);
        }
        return result;
    }

}
