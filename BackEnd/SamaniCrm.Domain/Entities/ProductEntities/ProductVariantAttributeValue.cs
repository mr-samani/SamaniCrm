using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductVariantAttributeValue
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public Guid AttributeId { get; set; }

        public string Value { get; set; } = default!;

        public ProductVariant ProductVariant { get; set; } = default!;
        public ProductAttribute Attribute { get; set; } = default!;
    }

}
