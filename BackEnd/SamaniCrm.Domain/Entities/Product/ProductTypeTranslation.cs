using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductTypeTranslation : BaseTranslation,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid ProductTypeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    [ForeignKey(nameof(ProductTypeId))]
    public virtual ProductType ProductType { get; set; }= default!;


}
