using Azure.Core;
using Hangfire;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
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
    private readonly IMasterDbContext _masterDbContext;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly IRecurringJobManagerV2 _JobManagerV2;
    private readonly IIdentityService _identityService;


    public TenantService(IApplicationDbContext dbContext,
        ITenantProvisioningService provisioningService,
        ICurrentUserService currentUser,
        ITenantNotificationService notificationService,
        ILogger<CreateTenantCommandHandler> logger,
        IRecurringJobManagerV2 recurringJobManagerV2,
        IIdentityService identityService,
        IMasterDbContext masterDbContext)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
        _JobManagerV2 = recurringJobManagerV2;
        _identityService = identityService;
        _masterDbContext = masterDbContext;
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

            await _masterDbContext.Tenants.AddAsync(tenant, cancellation);
            await _masterDbContext.SaveChangesAsync(cancellation);

            // TODO:Pasword must be hash for security save job
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

            var jobId = $"Create-Tenant-{tenant.Id}";

            //  Fire-and-Forget Job
            //BackgroundJob.Enqueue<ICreateTenantJobService>(
            //    job => job.ProvisioningTenantDependenciesAsync(serializedData, cancellation)
            //);

            // Scheduling 
            //BackgroundJob.Schedule<ICreateTenantJobService>(
            //    job => job.ProvisioningTenantDependenciesAsync(serializedData, cancellation),
            //    TimeSpan.FromSeconds(30)
            //);
            RecurringJob.AddOrUpdate<ICreateTenantJobService>(jobId,
                 job => job.ProvisioningTenantDependenciesAsync(serializedData, cancellation),
               Cron.Never,
               new RecurringJobOptions { MisfireHandling = MisfireHandlingMode.Relaxed });

            // ─── اجرا بعد از 10 ثانیه ───
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellation);
                RecurringJob.TriggerJob(jobId);
            }, cancellation);
            // RecurringJob.TriggerJob(jobId);

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

        var tenant = await _masterDbContext.Tenants.FindAsync(id, cancellation)
            ?? throw new NotFoundException("Tenant not found");

        tenant.Status = isSusspend ? TenantStatus.Suspended : TenantStatus.Active;
        tenant.SuspendedAt = isSusspend ? DateTime.UtcNow : null;
        tenant.SuspensionReason = reason;

        var result = await _masterDbContext.SaveChangesAsync(cancellation);
        return result > 0;
    }





    public async Task<bool> RetryProvisioning(Guid id, CancellationToken cancellation)
    {

        var tenant = await _masterDbContext.Tenants
            .Select(s => new
            {
                tenantId = s.Id,
                slug = s.Slug,
                address = s.Address
            })
            .Where(x => x.tenantId == id)
            .FirstOrDefaultAsync(cancellation);

        if (tenant == null)
        {
            throw new NotFoundException("Tenant not found");
        }

        var jobId = $"Create-Tenant-{tenant.tenantId}";
        string job = RecurringJob.TriggerJob(jobId);

        // RecurringJob.RemoveIfExists(jobId);

        return true;
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



    public async Task<List<AutoCompleteDto<Guid>>> GetTenantsAutoComplete(string? filter, CancellationToken cancellationToken)
    {
        var query = _masterDbContext.Tenants
             .AsNoTracking()
             .Where(x => x.Status == TenantStatus.Active);
        if (filter != null)
        {
            query = query.Where(x =>
                                 x.Slug.ToLower().Contains(filter.ToLower()) == true ||
                                 x.Name.ToLower().Contains(filter.ToLower()) == true
                                 );
        }

        var result = await query
             .OrderBy(x => x.Name)
             .Skip(0)
             .Take(200).Select(s => new AutoCompleteDto<Guid>()
             {
                 Id = s.Id,
                 Title = s.Name
             })
             .ToListAsync(cancellationToken);

        return result;
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