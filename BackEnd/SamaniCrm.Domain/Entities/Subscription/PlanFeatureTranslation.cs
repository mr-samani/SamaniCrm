using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class PlanFeatureTranslation : BaseTranslation, IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid PlanFeatureId { get; set; }
    public Guid PlanId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }
    public virtual PlanFeature PlanFeature { get; set; } = default!;
    public virtual Plan Plan { get; set; } = default!;

}