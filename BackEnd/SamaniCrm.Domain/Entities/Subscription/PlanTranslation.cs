using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class PlanTranslation : BaseTranslation, IMayHaveTenant

{
    public Guid? TenantId { get; set; }
    public Guid PlanId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }


    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual Plan Plan { get; set; } = default!;
}