using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.Features.Tenants.Dtos;

public class ProvisioningStatusDto
{
    public Guid TenantId { get; set; }
    public TenantProvisionStepsEnum Step { get; set; }
    public ProvisioningStepStatus StepStatus { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}



