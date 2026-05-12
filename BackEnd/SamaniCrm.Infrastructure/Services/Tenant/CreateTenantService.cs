using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Commands;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.Services.TenantService;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITenantProvisioningService _provisioningService;
    private readonly ICurrentUserService _currentUser;
    private readonly ITenantNotificationService _notificationService;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly IIdentityService _identityService;


    public TenantService(IApplicationDbContext dbContext,
        ITenantProvisioningService provisioningService,
        ICurrentUserService currentUser,
        ITenantNotificationService notificationService,
        ILogger<CreateTenantCommandHandler> logger,
        IIdentityService identityService)
    {
        _dbContext = dbContext;
        _provisioningService = provisioningService;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _logger = logger;
        _identityService = identityService;
    }



    /// <summary>
    /// Create tenant in background
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<CreateTenantResponse> CreateTenantAsync(CreateTenantCommand request, CancellationToken cancellation)
    {
        // Start provisioning tracking
        var provisioningSteps = GetInitialProvisioningSteps();
        try
        {
            // Step 1: Create Tenant Entity
            await _notificationService.SendProgressAsync(request.Slug, "Creating tenant entity...", 1, provisioningSteps, cancellation);

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
                ProvisioningStatus = ProvisioningStatus.Creating,
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

            // Step 2: Create Admin User
            await _notificationService.SendProgressAsync(
                request.Slug, "Creating admin user...", 2, provisioningSteps, cancellation);


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
                Roles = ["admin"],
            };
            var createUserResult = await _identityService.CreateUserAsync(adminUser);
            if (createUserResult.isSucceed == false)
            {
                throw new Exception("Can not Create AdminUser for tenant");
            }
            var adminUserId = createUserResult.userId;

            // Step 3: Create Default Roles
            await _notificationService.SendProgressAsync(
                request.Slug, "Creating default roles...", 3, provisioningSteps, cancellation);



            // Step 4: Link Admin to Tenant


            // Step 5: Provision Database (if isolated)
            if (request.DatabaseStrategy == DatabaseStrategy.Isolated)
            {
                await _notificationService.SendProgressAsync(
                    request.Slug, "Creating isolated database...", 4, provisioningSteps, cancellation);

                await _provisioningService.ProvisionIsolatedDatabaseAsync(tenant, cancellation);
            }

            // Step 6: Run Migrations
            await _notificationService.SendProgressAsync(
                request.Slug, "Running database migrations...", 5, provisioningSteps, cancellation);

            await _provisioningService.RunMigrationsAsync(tenant, cancellation);

            // Step 7: Seed Initial Data
            await _notificationService.SendProgressAsync(
                request.Slug, "Seeding initial data...", 6, provisioningSteps, cancellation);

            await _provisioningService.SeedInitialDataAsync(tenant, cancellation);

            // Step 8: Complete Provisioning
            await _notificationService.SendProgressAsync(
                request.Slug, "Finalizing setup...", 7, provisioningSteps, cancellation);

            tenant.ProvisioningStatus = ProvisioningStatus.Ready;
            tenant.Status = TenantStatus.Active;
            tenant.ModifiedAt = DateTime.UtcNow;
            tenant.ModifiedBy = _currentUser.UserId;

            await _dbContext.SaveChangesAsync(cancellation);

            // Send completion notification
            await _notificationService.SendCompletionAsync(
                request.Slug,
                "Tenant created successfully!",
                tenant.Id,
                adminUserId,
                cancellation);

            _logger.LogInformation(
                "Tenant {TenantId} ({Slug}) created successfully with admin {AdminId}",
                tenant.Id, tenant.Slug, adminUserId);

            return new CreateTenantResponse
            {
                TenantId = tenant.Id,
                AdminUserId = adminUserId,
                Slug = tenant.Slug,
                Status = tenant.Status.ToString(),
                ProvisioningStatus = tenant.ProvisioningStatus,
                TrialEndsAt = tenant.TrialEndsAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant {Slug}", request.Slug);

            // Update provisioning status
            var tenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Slug == request.Slug, cancellation);

            if (tenant != null)
            {
                tenant.ProvisioningStatus = ProvisioningStatus.Failed;
                tenant.ProvisioningError = ex.Message;
                await _dbContext.SaveChangesAsync(cancellation);
            }

            await _notificationService.SendErrorAsync(
                request.Slug,
                "Failed to create tenant: " + ex.Message,
                cancellation);

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









    /*-------------------------------------------------------------------------------------------------------*/
    private List<ProvisioningStep> GetInitialProvisioningSteps() => new()
{
    new() { Name = "CreateTenant", Description = "ایجاد موجودیت بهره‌بردار", Status = ProvisioningStepStatus.Pending },
    new() { Name = "CreateAdminUser", Description = "ایجاد کاربر مدیر", Status = ProvisioningStepStatus.Pending },
    new() { Name = "CreateRoles", Description = "ایجاد نقش‌های پیش‌فرض", Status = ProvisioningStepStatus.Pending },
    new() { Name = "ProvisionDatabase", Description = "ایجاد دیتابیس", Status = ProvisioningStepStatus.Pending },
    new() { Name = "RunMigrations", Description = "اجرای مایگریشن‌ها", Status = ProvisioningStepStatus.Pending },
    new() { Name = "SeedData", Description = "بارگذاری داده‌های اولیه", Status = ProvisioningStepStatus.Pending },
    new() { Name = "Finalize", Description = "تکمیل تنظیمات", Status = ProvisioningStepStatus.Pending }
};

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

    private string GetAllPermissions() => JsonSerializer.Serialize(new[]
    {
    "*" // All permissions
});

    private string GetAdminPermissions() => JsonSerializer.Serialize(new[]
    {
    "users.view", "users.create", "users.edit", "users.delete",
    "roles.view", "roles.create", "roles.edit",
    "settings.view", "settings.edit",
    "reports.view", "reports.export",
    "audit.view"
});

    private string GetBasicPermissions() => JsonSerializer.Serialize(new[]
    {
    "dashboard.view",
    "profile.view", "profile.edit"
});

    private string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
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
}