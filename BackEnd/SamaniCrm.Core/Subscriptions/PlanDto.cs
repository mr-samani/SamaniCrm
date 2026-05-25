using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Core.Shared.Subscriptions;

public class PlanDto
{
    public Guid? Id { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }
    [MaxLength(100)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    public BillingType BillingType { get; set; } = BillingType.Recurring;

    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public List<PlanTranslationDto> Translations { get; set; } = default!;
}
public class PlanTranslationDto
{
    public Guid? PlanId { get; set; }
    public required string Culture { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}