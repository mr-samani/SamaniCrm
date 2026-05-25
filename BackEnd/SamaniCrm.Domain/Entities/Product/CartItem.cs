using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class CartItem : BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public string? ProductName { get; set; }

    public virtual required Cart Cart { get; set; }

}
