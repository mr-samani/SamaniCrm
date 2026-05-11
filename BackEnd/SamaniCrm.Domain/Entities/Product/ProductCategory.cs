using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public Guid? ParentId { get; set; }

    public string Slug { get; set; } = default!;
    public string? Image { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ProductCategory? Parent { get; set; }
    public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<ProductCategoryTranslation> Translations { get; set; } = new List<ProductCategoryTranslation>();


}
