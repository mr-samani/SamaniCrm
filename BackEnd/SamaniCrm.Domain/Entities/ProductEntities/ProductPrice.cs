using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductPrice
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }

        public string Currency { get; set; } = default!;  // مثلا "USD", "IRR"
        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ProductVariant ProductVariant { get; set; } = default!;
    }

}
