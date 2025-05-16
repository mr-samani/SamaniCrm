using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.ValueObjects.Product;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductTypeId { get; set; }

        // public string SKU { get; set; } = default!;
        private Sku _sku;

        public Sku SKU
        {
            get => _sku;
            private set => _sku = value ?? throw new ArgumentNullException(nameof(SKU));
        }


        public string Title { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public Tenant Tenant { get; set; } = default!;
        public ProductCategory Category { get; set; } = default!;
        public ProductType ProductType { get; set; } = default!;

        public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }

}
