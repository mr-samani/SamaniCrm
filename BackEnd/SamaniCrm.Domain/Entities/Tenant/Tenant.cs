using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;


public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    
    // Contact
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Website { get; set; }
    
    // Address
    public string Country { get; set; } = "Iran";
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Database Configuration
    public DatabaseStrategy DatabaseStrategy { get; set; } = DatabaseStrategy.Shared;
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? ServerName { get; set; }
    
    // Branding
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";
    public string? FaviconUrl { get; set; }
    
    // Subscription
    public string SubscriptionPlan { get; set; } = "Basic";
    public int MaxUsers { get; set; } = 10;
    public long MaxStorageMB { get; set; } = 1024;
    public long? MaxApiCallsPerMonth { get; set; }
    public long CurrentStorageMB { get; set; } = 0;
    
    // Status
    public TenantStatus Status { get; set; } = TenantStatus.Pending;
    public bool IsTrial { get; set; } = true;
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? SubscriptionStartsAt { get; set; }
    public DateTime? SubscriptionEndsAt { get; set; }
    
    // Provisioning
    public ProvisioningStatus ProvisioningStatus { get; set; } = ProvisioningStatus.NotStarted;
    public List<ProvisioningStep>? ProvisioningSteps { get; set; }
    public string? ProvisioningError { get; set; }
    
    // Security
    public List<string>? AllowedIpAddresses { get; set; }
    public bool Require2FA { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int PasswordMinLength { get; set; } = 8;
    public bool PasswordRequireSpecialChar { get; set; } = true;
    
    // Feature Flags
    public Dictionary<string, bool>? FeatureFlags { get; set; }


    // susspend
    public DateTime? SuspendedAt { get; set; }

    [MaxLength(2000)]
    public string? SuspensionReason { get; set; }



    // Navigation
    public virtual ICollection<TenantSetting> Settings { get; set; } = new List<TenantSetting>();
    public virtual ICollection<TenantDatabaseConnection> DatabaseConnections { get; set; } = new List<TenantDatabaseConnection>();
    
    // Computed
    public bool IsActive => Status == TenantStatus.Active;
    public bool IsSuspended => Status == TenantStatus.Suspended;
    public bool IsTrialExpired => IsTrial && TrialEndsAt.HasValue && TrialEndsAt.Value < DateTime.UtcNow;

 
}
