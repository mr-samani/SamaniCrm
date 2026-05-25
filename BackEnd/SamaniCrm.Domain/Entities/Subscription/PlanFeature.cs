using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class PlanFeature:BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid PlanId { get; set; }
    [MaxLength(100)]
    public required string FeatureKey { get; set; } // مثلاً: "api_calls", "storage"
    [MaxLength(200)]
    public required string DisplayName { get; set; }
    public PlanFeatureType PlanFeatureType { get; set; }
    [MaxLength(250)]
    public required string Value { get; set; } // -- مقدار: عدد، متن، یا "unlimited"
    [MaxLength(50)]
    public required string Unit { get; set; } //-- واحد: "requests", "GB", "users"
    public int SortOrder { get; set; } = 0;


    public virtual Plan Plan { get; set; } = default!;
}
