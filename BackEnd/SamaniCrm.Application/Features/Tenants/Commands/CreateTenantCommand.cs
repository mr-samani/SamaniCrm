using MediatR;
using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants;


public class CreateTenantCommand : IRequest<CreateTenantResponse>
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase alphanumeric with hyphens")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(300)]
    public string? LegalName { get; set; }

    [StringLength(50)]
    public string? RegistrationNumber { get; set; }

    [StringLength(50)]
    public string? TaxId { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Phone]
    public string? Mobile { get; set; }

    [StringLength(200)]
    public string? Website { get; set; }

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = "Iran";

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; } = default!;
    public decimal? Longitude { get; set; } = default!;

    public DatabaseStrategy DatabaseStrategy { get; set; } = DatabaseStrategy.Shared;

    // Admin User Data
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AdminFirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string AdminLastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string AdminUserName { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string AdminPassword { get; set; } = string.Empty;

    [Phone]
    public required string AdminMobile { get; set; }


    // Subscription
    public string SubscriptionPlan { get; set; } = "Basic";
    public int MaxUsers { get; set; } = 10;
    public long MaxStorageMB { get; set; } = 1024;
    public bool IsTrial { get; set; } = true;
    public int TrialDays { get; set; } = 14;

    // Branding
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";

    // Security Settings
    public bool Require2FA { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int PasswordMinLength { get; set; } = 8;
    public bool PasswordRequireSpecialChar { get; set; } = true;
    public List<string>? AllowedIpAddresses { get; set; }

    // Feature Flags
    public Dictionary<string, bool>? FeatureFlags { get; set; }
}
