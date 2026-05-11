using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace SamaniCrm.Core.Shared.Interfaces.Tenant;

public interface ITenant
{
    Guid Id { get; set; }
    string Name { get; set; }
    string Slug { get; set; }
    string Email { get; set; }
    TenantStatus Status { get; set; }
    bool Require2FA { get; set; }
    int SessionTimeoutMinutes { get; set; }
    Dictionary<string, bool>? FeatureFlags { get; set; }
    bool IsActive { get; }


    // Database Configuration
    DatabaseStrategy DatabaseStrategy { get; set; }
    string? ConnectionString { get; set; }
    string? DatabaseName { get; set; }
    string? ServerName { get; set; }


    // Security
    List<string>? AllowedIpAddresses { get; set; }


    DateTime? UpdatedAt { get; set; }
    string? SuspensionReason { get; set; }
    DateTime? SuspendedAt { get; set; }
}
