using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class AddOnTranslation : BaseTranslation, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    public Guid AddOnId { get; set; }

    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
    public decimal UnitPrice { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }


    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual AddOn AddOn { get; set; } = default!;

}