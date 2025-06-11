using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public string? ProductName { get; set; }

        public virtual required Cart Cart { get; set; } 

    }
}
