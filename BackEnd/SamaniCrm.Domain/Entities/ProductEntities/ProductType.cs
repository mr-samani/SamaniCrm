using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductType:IAuditableEntity,ISoftDelete
    {
        public Guid Id { get; set; }

        public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public virtual ICollection<ProductTypeTranslation> Translations { get; set; } = new List<ProductTypeTranslation>();




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
