using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string Url { get; set; } = default!;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }

        public Product Product { get; set; } = default!;
    }

}
