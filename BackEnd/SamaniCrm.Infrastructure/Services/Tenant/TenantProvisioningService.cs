using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Persistence;
using HealthStatus = SamaniCrm.Domain.Entities.HealthStatus;


namespace SamaniCrm.Infrastructure.Services.TenantService;


public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly MasterDbContext _dbContext;
    private readonly ITenantDatabaseService _databaseService;
    private readonly ILogger<TenantProvisioningService> _logger;
    private readonly ApplicationDbInitializer _dbInitializer;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    public TenantProvisioningService(
        ITenantDatabaseService databaseService,
        ILogger<TenantProvisioningService> logger,
        MasterDbContext dbContext,
        IIdentityService identityService,
        ApplicationDbInitializer dbInitializer,
        ITenantDbContextFactory tenantDbContextFactory)
    {
        _databaseService = databaseService;
        _logger = logger;
        _dbContext = dbContext;
        _dbInitializer = dbInitializer;
        _tenantDbContextFactory = tenantDbContextFactory;
    }



    /// <summary>
    /// ایجاد مراحل آماده سازی بهره بردار
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
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

    /// <summary>
    /// دریافت آخرین مرحله اجرا نشده
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
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




    /// <summary>
    /// ایجاد مدیر بهره بردار
    /// </summary>
    /// <param name="request"></param>
    /// <param name="connectionString"></param>
    /// <param name="tenantId"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task ProvisionCreateAdminUser(TenantJobProvisioningData request, string connectionString, Guid tenantId, CancellationToken cancellation)
    {
        await using var dbContext = _tenantDbContextFactory.Create(connectionString);

        var exists = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId &&
                     x.UserName == request.AdminUserName,
                cancellation);

        if (exists != null)
            return;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,

            UserName = request.AdminUserName,
            NormalizedUserName = request.AdminUserName.ToUpperInvariant(),

            Email = request.AdminEmail.ToLowerInvariant(),
            NormalizedEmail = request.AdminEmail.ToUpperInvariant(),

            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            PhoneNumber = request.AdminMobile,
            Address = request.Address,

            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = true,

            Lang = AppConsts.DefaultLanguage,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        user.PasswordHash = passwordHasher.HashPassword(user, request.AdminPassword);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellation);

        var tenantAdminRole = await dbContext.Roles
            .IgnoreQueryFilters()
            .Where(x => x.Name == AppRoles.TenantAdministrator && x.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellation);

        if (tenantAdminRole == null)
        {
            throw new Exception($"Role '{AppRoles.TenantAdministrator}' not found.");
        }

        dbContext.UserRoles.Add(new()
        {
            UserId = user.Id,
            RoleId = tenantAdminRole.Id
        });

        await dbContext.SaveChangesAsync(cancellation);

    }



    /// <summary>
    /// ایجاد دیتابیس مجزا برای بهره  بردار
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
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


    /// <summary>
    /// اجرای مایگریشن ها روی دیتابیس
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation)
    {
        if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated && !string.IsNullOrEmpty(tenant.ConnectionString))
        {
            var connectionString = _databaseService.DecryptConnectionString(tenant.ConnectionString);
            await _databaseService.RunMigrationsAsync(connectionString, tenant.Id, cancellation);
        }
        else
        {
            // TODO: No nedd run migrations for shared db
            await _dbContext.Database.MigrateAsync(cancellation);
        }
    }


    /// <summary>
    /// پر کردن داده های لازم در دریتابیس بهره بردار
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation)
    {
        await _dbInitializer.SeedAsync(tenant);
    }


    /// <summary>
    /// مرحله نهایی آماده سازی بهره بردار
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
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



    /// <summary>
    /// بررسی وضعیت اتضال به دیتابیس بهره بردار
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation)
    {
        return await _databaseService.TestConnectionAsync(connectionString, cancellation);
    }





    /// <summary>
    /// دریافت آخرین مرحله انجام شده برای اماده سازی بهره بردار
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="cancellation"></param>
    /// <param name="ChackInit"></param>
    /// <returns></returns>
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
