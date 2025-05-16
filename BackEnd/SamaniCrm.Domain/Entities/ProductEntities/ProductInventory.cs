using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductInventory
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }

        public int Quantity { get; set; }
        public Guid? WarehouseId { get; set; } // اگر انبار تعریف شده باشد

        public DateTime LastUpdated { get; set; }

        public ProductVariant ProductVariant { get; set; } = default!;
    }

}
