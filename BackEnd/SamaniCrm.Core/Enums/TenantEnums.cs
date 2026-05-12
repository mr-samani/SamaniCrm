using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Enums;

public enum TenantStatus
{
    Pending = 1,
    Active = 2,
    Suspended = 3,
    Cancelled = 4,
    Deleted = -1
}
public enum DatabaseStrategy
{
    Shared = 1,      // All tenants in one database
    Isolated = 2     // Each tenant has its own database
}
public enum ProvisioningStatus
{
    NotStarted = 0,
    Creating = 1,
    Ready = 2,
    Failed = 3
}

public class ProvisioningStep
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProvisioningStepStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum ProvisioningStepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}


public enum TenantSettingValueType
{
    String = 1,
    Number,
    Boolean,
    JSON
}
public enum ProvisioningNotificationStatus
{
    InProgress = 1,
    Completed = 2,
    Failed = 3
}