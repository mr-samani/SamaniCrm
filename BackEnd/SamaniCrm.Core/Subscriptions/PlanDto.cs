using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Core.Shared.Subscriptions;

public class PlanDto
{
    public Guid? Id { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    public BillingType BillingType { get; set; } = BillingType.Recurring;

    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}
