using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class ProductAttribute : BaseEntity,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid ProductTypeId { get; set; }

    public ProductAttributeDataTypeEnum DataType { get; set; } = default!; // string, int, decimal, bool...
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int SortOrder { get; set; }

    [ForeignKey(nameof(ProductTypeId))]
    public virtual ProductType ProductType { get; set; } = default!;
    public virtual ICollection<ProductAttributeTranslation> Translations { get; set; } = new List<ProductAttributeTranslation>();
    public virtual ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();

}




