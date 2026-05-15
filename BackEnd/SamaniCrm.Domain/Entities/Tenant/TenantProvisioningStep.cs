using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;


namespace SamaniCrm.Domain.Entities;

public class TenantProvisioningStep
{
    [Key]
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    public ProvisioningStepStatus Status { get; set; } = ProvisioningStepStatus.Pending;
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}


