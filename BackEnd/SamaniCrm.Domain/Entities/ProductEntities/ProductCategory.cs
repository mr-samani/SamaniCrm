using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductCategory
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public Tenant Tenant { get; set; } = default!;
        public ProductCategory? Parent { get; set; }
        public ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
