using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }

        public string? DiscountCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



        public virtual ICollection<CartItem> CartItems { get; set; } = [];



    }
}
