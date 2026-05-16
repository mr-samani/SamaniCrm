using Hangfire;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Commands;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Jobs;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Services.TenantService;


public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly IRecurringJobManagerV2 _JobManagerV2;


    public TenantService(IApplicationDbContext dbContext,
        ITenantProvisioningService provisioningService,
        ICurrentUserService currentUser,
        ITenantNotificationService notificationService,
        ILogger<CreateTenantCommandHandler> logger,
        IIdentityService identityService,
        IRecurringJobManagerV2 recurringJobManagerV2)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
        _JobManagerV2 = recurringJobManagerV2;
    }



    /// <summary>
    /// Create tenant in background
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<CreateTenantResponse> CreateTenantAsync(CreateTenantCommand request, CancellationToken cancellation)
    {
        try
        {

            var tenant = new Tenant
            {
                Name = request.Name,
                Slug = request.Slug.ToLowerInvariant(),
                LegalName = request.LegalName,
                RegistrationNumber = request.RegistrationNumber,
                TaxId = request.TaxId,
                Email = request.Email.ToLowerInvariant(),
                Phone = request.Phone,
                Mobile = request.Mobile,
                Website = request.Website,
                Country = request.Country,
                City = request.City,
                Address = request.Address,
                PostalCode = request.PostalCode,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                DatabaseStrategy = request.DatabaseStrategy,
                LogoUrl = request.LogoUrl,
                PrimaryColor = request.PrimaryColor,
                SecondaryColor = request.SecondaryColor,
                SubscriptionPlan = request.SubscriptionPlan,
                MaxUsers = request.MaxUsers,
                MaxStorageMB = request.MaxStorageMB,
                IsTrial = request.IsTrial,
                TrialEndsAt = request.IsTrial ? DateTime.UtcNow.AddDays(request.TrialDays) : null,
                Status = TenantStatus.Pending,
                ProvisioningStatus = ProvisioningStatus.NotStarted,
                Require2FA = request.Require2FA,
                SessionTimeoutMinutes = request.SessionTimeoutMinutes,
                PasswordMinLength = request.PasswordMinLength,
                PasswordRequireSpecialChar = request.PasswordRequireSpecialChar,
                AllowedIpAddresses = request.AllowedIpAddresses,
                FeatureFlags = request.FeatureFlags ?? GetDefaultFeatureFlags(),
                CreatedBy = _currentUser.UserId
            };

            await _dbContext.Tenants.AddAsync(tenant, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);



            TenantJobProvisioningData jobData = new TenantJobProvisioningData()
            {
                TenantId = tenant.Id,
                Slug = request.Slug,
                AdminEmail = request.AdminEmail.ToLowerInvariant(),
                AdminFirstName = request.AdminFirstName,
                AdminLastName = request.AdminLastName,
                AdminPassword = request.AdminPassword,
                AdminUserName = request.AdminUserName,
                AdminMobile = request.AdminMobile,
                Address = request.Address ?? ""
            };



            var serializedData = JsonSerializer.Serialize(jobData);


            //  Fire-and-Forget Job
            var jobId = BackgroundJob.Enqueue<ICreateTenantJobService>(
                job => job.ProvisioningTenantDependenciesAsync(serializedData, cancellation)
            );





            return new CreateTenantResponse
            {
                TenantId = tenant.Id,
                Slug = tenant.Slug,
                Status = tenant.Status.ToString(),
                ProvisioningStatus = tenant.ProvisioningStatus,
                TrialEndsAt = tenant.TrialEndsAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant {Slug}", request.Slug);
            throw;
        }

    }



    /// <summary>
    /// suspend or active tenant
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<bool> ActiveOrDeactiveTenant(Guid id, bool isSusspend, string? reason, CancellationToken cancellation)
    {

        var tenant = await _dbContext.Tenants.FindAsync(id, cancellation)
            ?? throw new NotFoundException("Tenant not found");

        tenant.Status = isSusspend ? TenantStatus.Suspended : TenantStatus.Active;
        tenant.SuspendedAt = isSusspend ? DateTime.UtcNow : null;
        tenant.SuspensionReason = reason;

        var result = await _dbContext.SaveChangesAsync(cancellation);
        return result > 0;
    }





    public async Task<bool> RetryProvisioning(Guid id, CancellationToken cancellation)
    {

        var tenant = await _dbContext.Tenants.FindAsync(id, cancellation)
                    ?? throw new NotFoundException("Tenant not found");
        throw new NotImplementedException();


        //tenant.Provisioning.RetryCount++;
        //tenant.Provisioning.Status = ProvisioningStatus.Pending;
        //tenant.Provisioning.ErrorMessage = null;
        //tenant.UpdatedAt = DateTime.UtcNow;

        //await _unitOfWork.SaveChangesAsync(cancellation);
        //_ = Task.Run(() => _provisioningService.ProvisionAsync(tenant.Id));

        //return bool.Value;
    }

    public async Task<bool> UpdateTenant(UpdateTenantSettingsCommand request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _dbContext.Tenants.FindAsync(request.TenantId, cancellation)
        //             ?? throw new NotFoundException("Tenant not found");

        //if (request.TimeZone != null) tenant.Settings.TimeZone = request.TimeZone;
        //if (request.Currency != null) tenant.Settings.Currency = request.Currency;
        //if (request.Language != null) tenant.Settings.Language = request.Language;
        //if (request.MaxUsers.HasValue) tenant.Settings.MaxUsers = request.MaxUsers.Value;
        //if (request.MaxStorageMb.HasValue) tenant.Settings.MaxStorageMb = request.MaxStorageMb.Value;
        //if (request.AllowCustomBranding.HasValue) tenant.Settings.AllowCustomBranding = request.AllowCustomBranding.Value;
        //if (request.CustomSettings != null) tenant.Settings.CustomSettings = request.CustomSettings;

        //var result = await _dbContext.SaveChangesAsync(cancellation);
        //return result > 0;
    }








    /*-------------------------------------------------------------------------------------------------------*/


    private Dictionary<string, bool> GetDefaultFeatureFlags() => new()
    {
        ["dashboard"] = true,
        ["reports"] = true,
        ["api_access"] = true,
        ["sso"] = false,
        ["audit_logs"] = true,
        ["file_storage"] = true,
        ["notifications"] = true
    };


}