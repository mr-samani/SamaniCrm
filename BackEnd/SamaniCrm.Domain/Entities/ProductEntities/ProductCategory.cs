using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductCategory:IAuditableEntity , ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? ParentId { get; set; }

        public string Slug { get; set; } = default!;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public virtual Tenant Tenant { get; set; } = default!;
        public virtual ProductCategory? Parent { get; set; }
        public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
       // public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<ProductCategoryTranslation> Translations { get; set; } = new List<ProductCategoryTranslation>();




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
