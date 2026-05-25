using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities.Subscription;

public class AddOn:BaseEntity, IMayHaveTenant
{
    public Guid? TenantId { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string Code { get; set; }
    [MaxLength(500)]    
    public string? Description { get; set; }
    public BillingType BillingType { get; set; }

    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }
}

