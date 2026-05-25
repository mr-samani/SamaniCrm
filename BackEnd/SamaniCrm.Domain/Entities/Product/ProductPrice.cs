using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class ProductPrice : BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid ProductId { get; set; }

    [MaxLength(5)]
    public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
    public decimal Price { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PriceTypeEnum Type { get; set; }

    public virtual Product Product { get; set; } = default!;
    public virtual Currency Currency { get; set; } = default!;

}



