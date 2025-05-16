using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductAttribute
    {
        public Guid Id { get; set; }
        public Guid ProductTypeId { get; set; }

        public string Name { get; set; } = default!;
        public string DataType { get; set; } = default!; // string, int, decimal, bool...
        public bool IsRequired { get; set; }
        public bool IsVariant { get; set; }
        public int SortOrder { get; set; }

        public ProductType ProductType { get; set; } = default!;
    }

}
