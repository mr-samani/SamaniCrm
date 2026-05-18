using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System.Security.Cryptography;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Jobs;


public interface ICreateTenantJobService
{
    Task ProvisioningTenantDependenciesAsync(string jobData, CancellationToken cancellation);
}
public class CreateTenantJobService : ICreateTenantJobService
{

    private readonly ITenantNotificationService _notificationService;
    private readonly ITenantProvisioningService _provisioningService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateTenantJobService> _logger;


    private readonly int StepDelay = 10;
    private TenantJobProvisioningData? _jobData;
    private TenantProvisionStepsEnum _currentStep = TenantProvisionStepsEnum.CreateTenant;

    public CreateTenantJobService(IIdentityService identityService,
        ITenantNotificationService notificationService,
        ITenantProvisioningService provisioningService,
        IApplicationDbContext dbContext,
        ILogger<CreateTenantJobService> logger)
    {
        _notificationService = notificationService;
        _provisioningService = provisioningService;
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task ProvisioningTenantDependenciesAsync(string serializedJobData, CancellationToken cancellation)
    {

        _jobData = JsonSerializer.Deserialize<TenantJobProvisioningData>(serializedJobData)!;

        // بررسی آیا این tenant در حال پردازش هست
        var tenant = await _dbContext.Tenants
            .Where(x => x.Id == _jobData.TenantId)
            .FirstOrDefaultAsync(cancellation);

        if (tenant == null)
        {
            throw new NotFoundException("Tenant not found!");
        }

        if (tenant.ProvisioningStatus == ProvisioningStatus.InProgress)
        {
            _logger.LogWarning("Tenant {TenantId} is already being provisioned", _jobData.TenantId);
            return;
        }

        // قفل کردن
        tenant.ProvisioningStatus = ProvisioningStatus.InProgress;
        await _dbContext.SaveChangesAsync(cancellation);

        try
        {
            await _provisioningService.InitializeTenantProvisionSteps(tenant.Id, cancellation);
            var pendingSteps = await _provisioningService.GetPendingProvisionSteps(tenant.Id, cancellation);

            _currentStep = TenantProvisionStepsEnum.CreateTenant;


            // Start provisioning tracking
            await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");

            // Step 1: Create Admin User
            if (pendingSteps.Contains(TenantProvisionStepsEnum.CreateAdminUser))
            {
                _currentStep = TenantProvisionStepsEnum.CreateAdminUser;
                await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress, "");
                await Task.Delay(TimeSpan.FromSeconds(StepDelay));
                await _provisioningService.ProvisionCreateAdminUser(_jobData, tenant.Id, cancellation);

                await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");
            }


            // Step 2: Provision Database (if isolated)
            if (pendingSteps.Contains(TenantProvisionStepsEnum.ProvisionDatabase))
            {
                _currentStep = TenantProvisionStepsEnum.ProvisionDatabase;

                await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress, "");
                await Task.Delay(TimeSpan.FromSeconds(StepDelay));

                if (tenant.DatabaseStrategy == DatabaseStrategy.Isolated)
                {
                    await _provisioningService.ProvisionIsolatedDatabaseAsync(tenant, cancellation);
                }
                await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");

            }
            // Step 3: Run Migrations
            if (pendingSteps.Contains(TenantProvisionStepsEnum.RunMigrations))
            {
                _currentStep = TenantProvisionStepsEnum.RunMigrations;

                await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress, "");
                await Task.Delay(TimeSpan.FromSeconds(StepDelay));
                await _provisioningService.RunMigrationsAsync(tenant, cancellation);
                await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");
            }
            // Step 4: Seed Initial Data
            if (pendingSteps.Contains(TenantProvisionStepsEnum.SeedData))
            {
                _currentStep = TenantProvisionStepsEnum.SeedData;

                await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress, "");
                await Task.Delay(TimeSpan.FromSeconds(StepDelay));
                await _provisioningService.SeedInitialDataAsync(tenant, cancellation);
                await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");
            }


            // Step 5: Finalize
            if (pendingSteps.Contains(TenantProvisionStepsEnum.Finalize))
            {
                _currentStep = TenantProvisionStepsEnum.Finalize;

                await SendNotificationProcessAsync(ProvisioningStepStatus.InProgress, "");
                await Task.Delay(TimeSpan.FromSeconds(StepDelay));
                await _provisioningService.FinalizeAsync(tenant, cancellation);
                await SendNotificationProcessAsync(ProvisioningStepStatus.Completed, "");
            }


        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error on Create tenant {TenantId}:{Message}", tenant.Id, ex.Message);
            await SendNotificationProcessAsync(ProvisioningStepStatus.Failed, ex.Message);

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


    private async Task SendNotificationProcessAsync(ProvisioningStepStatus status, string? errorMessage)
    {
        if (_jobData == null)
        {
            return;
        }
        var tenantSlug = _jobData.Slug;
        var tenantId = _jobData.TenantId;

        var stepEntity = await _dbContext.TenantProvisioningSteps
               .Where(x => x.TenantId == tenantId && x.Step == _currentStep)
               .FirstOrDefaultAsync();


        switch (status)
        {
            // InProgress
            case ProvisioningStepStatus.InProgress:
                await _notificationService.SendProgressAsync(tenantSlug, _currentStep, "");
                if (stepEntity != null)
                {
                    stepEntity.StepStatus = ProvisioningStepStatus.InProgress;
                    stepEntity.StartedAt = DateTime.UtcNow;
                    stepEntity.CompletedAt = null;
                    await _dbContext.SaveChangesAsync();
                }
                break;

            // Failed
            case ProvisioningStepStatus.Failed:
                await _notificationService.SendErrorAsync(tenantSlug, _currentStep, errorMessage);
                if (stepEntity != null)
                {
                    stepEntity.StepStatus = ProvisioningStepStatus.Failed;
                    stepEntity.CompletedAt = null;

                    Tenant? tenant = await _dbContext.Tenants.Where(x => x.Id == tenantId).FirstOrDefaultAsync();
                    if (tenant != null)
                    {
                        tenant.ProvisioningStatus = ProvisioningStatus.Failed;
                    }
                    await _dbContext.SaveChangesAsync();

                }
                break;

            // Done
            case ProvisioningStepStatus.Completed:
                await _notificationService.SendCompletionAsync(tenantSlug, _currentStep, "");
                if (stepEntity != null)
                {
                    stepEntity.StepStatus = ProvisioningStepStatus.Completed;
                    stepEntity.CompletedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
                break;

        }
    }

}
