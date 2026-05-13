using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Infrastructure.Services.TenantService;

public class ProvisioningNotification
{
    public string TenantSlug { get; set; } = string.Empty;
    public ProvisioningNotificationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public TenantProvisionStepsEnum CurrentStep { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? AdminUserId { get; set; }
    public DateTime Timestamp { get; set; }
}
