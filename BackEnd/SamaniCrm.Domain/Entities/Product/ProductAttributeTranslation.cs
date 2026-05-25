using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductAttributeTranslation : BaseTranslation
{
    public Guid ProductAttributeId { get; set; }
    public string Name { get; set; } = default!;

    public virtual ProductAttribute ProductAttribute { get; set; } = default!;

}
