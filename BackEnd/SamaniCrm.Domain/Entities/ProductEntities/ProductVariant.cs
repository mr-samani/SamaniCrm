using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string SKU { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; } = default!;
        public ICollection<ProductVariantAttributeValue> VariantAttributes { get; set; } = new List<ProductVariantAttributeValue>();

        public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public ICollection<ProductInventory> Inventories { get; set; } = new List<ProductInventory>();


    }

}
