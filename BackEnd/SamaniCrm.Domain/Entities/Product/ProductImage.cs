using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductImage: BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid FileId { get; set; } 
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }

    public virtual Product Product { get; set; } = default!;
    public virtual FileFolder File { get; set; } = default!;


}
