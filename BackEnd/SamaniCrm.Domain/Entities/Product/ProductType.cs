using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductType : BaseEntity,IMayHaveTenant
{

    public Guid? TenantId { get; set; }
    public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ProductTypeTranslation> Translations { get; set; } = new List<ProductTypeTranslation>();

}
