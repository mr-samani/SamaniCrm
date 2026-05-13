using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Constants;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Services.TenantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Jobs;


public class TenantJobProvisioningData
{
    public Guid TenantId { get; set; }
    public required string Slug { get; set; }
    public required string  AdminEmail { get; set; }
    public required string  AdminFirstName { get; set; }
    public required string  AdminLastName { get; set; }
    public required string  AdminPassword { get; set; }
    public required string  AdminUserName { get; set; }
    public required string  AdminMobile { get; set; }
    public required string  Address { get; set; }
}


public interface ICreateTenantJobService
{
    Task ProvisioningTenantDependenciesAsync(string jobData, CancellationToken cancellation);
}
public class CreateTenantJobService : ICreateTenantJobService
{

    private readonly IIdentityService _identityService;
    private readonly ITenantNotificationService _notificationService;
    private readonly ITenantProvisioningService _provisioningService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateTenantJobService> _logger;


    public CreateTenantJobService(IIdentityService identityService,
        ITenantNotificationService notificationService,
        ITenantProvisioningService provisioningService,
        IApplicationDbContext dbContext,
        ILogger<CreateTenantJobService> logger)
    {
        _identityService = identityService;
        _notificationService = notificationService;
        _provisioningService = provisioningService;
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task ProvisioningTenantDependenciesAsync(string serializedJobData, CancellationToken cancellation)
    {

        var jobData = JsonSerializer.Deserialize<TenantJobProvisioningData>(serializedJobData)!;

        // بررسی آیا این tenant در حال پردازش هست
        var tenant = await _dbContext.Tenants
            .Where(x => x.Id == jobData.TenantId)
            .FirstOrDefaultAsync(cancellation);

        if (tenant == null)
        {
            throw new NotFoundException("Tenant not found!");
        }

        if (tenant.ProvisioningStatus == ProvisioningStatus.InProgress)
        {
            _logger.LogWarning("Tenant {TenantId} is already being provisioned", jobData.TenantId);
            return;
        }

        // قفل کردن
        tenant.ProvisioningStatus = ProvisioningStatus.InProgress;
        await _dbContext.SaveChangesAsync(cancellation);



        // Step : Create Tenant provission steps
        await _provisioningService.InitializeTenantProvisionSteps(tenant.Id, cancellation);
        // Start provisioning tracking
        await _notificationService.SendProgressAsync(jobData.Slug, "Creating tenant entity...", TenantProvisionStepsEnum.CreateTenant);


        var pendingSteps = await _provisioningService.GetPendingProvisionSteps(tenant.Id, cancellation);

        // Step : Create Admin User
        if (pendingSteps.Contains(TenantProvisionStepsEnum.CreateAdminUser))
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await _provisioningService.ProvisionCreateAdminUser(jobData, tenant.Id, cancellation);
        }
        await Task.Delay(TimeSpan.FromSeconds(30));


        // Step : Provision Database (if isolated)
        if (pendingSteps.Contains(TenantProvisionStepsEnum.ProvisionDatabase))
        {
            if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated)
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                await _provisioningService.ProvisionIsolatedDatabaseAsync(tenant, cancellation);
            }
        }
        // Step : Run Migrations
        if (pendingSteps.Contains(TenantProvisionStepsEnum.RunMigrations))
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await _provisioningService.RunMigrationsAsync(tenant, cancellation);
        }
        // Step : Seed Initial Data
        if (pendingSteps.Contains(TenantProvisionStepsEnum.SeedData))
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await _provisioningService.SeedInitialDataAsync(tenant, cancellation);
        }
        if (pendingSteps.Contains(TenantProvisionStepsEnum.Finalize))
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await _provisioningService.FinalizeAsync(tenant, cancellation);
        }




    }





    /*-------------------------------------------------------------------------------------------------------*/

    private string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
    }

}
