using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class AddOn : BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    [MaxLength(100)]
    public required string Code { get; set; }
    public BillingType BillingType { get; set; }


    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }

    public List<AddOnTranslation> Translations { get; set; } = new List<AddOnTranslation>();
}
