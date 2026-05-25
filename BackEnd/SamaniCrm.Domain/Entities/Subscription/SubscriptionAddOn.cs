using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class SubscriptionAddOn:BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid? SubscriptionId { get; set; }
    public Guid? AddOnId { get; set; }

    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }


    public virtual required Subscription Subscription { get; set; }
    public virtual required AddOn AddOn { get; set; }

}
