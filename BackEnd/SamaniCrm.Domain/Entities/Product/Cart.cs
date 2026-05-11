using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid? UserId { get; set; }

    public string? DiscountCode { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = [];

}
