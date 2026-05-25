using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;


namespace SamaniCrm.Domain.Entities;

public class TenantProvisioningStep
{
    [Key]
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public required TenantProvisionStepsEnum Step { get; set; }
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    public ProvisioningStepStatus StepStatus { get; set; } = ProvisioningStepStatus.Pending;

    public int RetryCount { get; set; } = 0;

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}


