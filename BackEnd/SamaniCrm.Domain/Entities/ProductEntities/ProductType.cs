using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductType
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public Tenant Tenant { get; set; } = default!;
        public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    }

}
