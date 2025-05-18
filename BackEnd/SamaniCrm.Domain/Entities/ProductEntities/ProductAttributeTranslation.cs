using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductAttributeTranslation:TranslationBaseClass,IAuditableEntity,ISoftDelete
    {
        public Guid ProductAttributeId { get; set; }
        public string Name { get; set; } = default!;

        public virtual ProductAttribute ProductAttribute { get; set; } = default!;


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
