using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductAttribute : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid ProductTypeId { get; set; }

        public ProductAttributeDataTypeEnum DataType { get; set; } = default!; // string, int, decimal, bool...
        public bool IsRequired { get; set; }
        public bool IsVariant { get; set; }
        public int SortOrder { get; set; }

        [ForeignKey(nameof(ProductTypeId))]
        public virtual ProductType ProductType { get; set; } = default!;
        public virtual ICollection<ProductAttributeTranslation> Translations { get; set; } = new List<ProductAttributeTranslation>();
        public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();



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
