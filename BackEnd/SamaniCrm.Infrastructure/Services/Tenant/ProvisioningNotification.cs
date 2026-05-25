using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Infrastructure.Services.TenantService;

public class ProvisioningNotification
{
    public required string TenantSlug { get; set; }
    public required ProvisioningStepStatus Status { get; set; }
    public string? Message { get; set; }
    public required TenantProvisionStepsEnum CurrentStep { get; set; }
    public required DateTime Timestamp { get; set; }
}
