using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Domain.ValueObjects.Product;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class Product : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public Sku SKU { get; set; } = default!;
        //private Sku _sku;

        //public Sku SKU
        //{
        //    get => _sku;
        //    private set => _sku = value ?? throw new ArgumentNullException(nameof(SKU));
        //}


        public string Slug { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public virtual Tenant Tenant { get; set; } = default!;
        public ProductCategory Category { get; set; } = default!;
        public virtual ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();


        public ProductType ProductType { get; set; } = default!;

        public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
        //public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        //public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();




        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

        // Implementing ISoftDelete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? DeletedBy { get; set; }


    }

}
