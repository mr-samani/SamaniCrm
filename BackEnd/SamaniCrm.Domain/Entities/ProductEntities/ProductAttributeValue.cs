using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.ValueObjects.Product;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductAttributeValue
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid AttributeId { get; set; }

        // public string Value { get; set; } = default!;
        private AttributeValue _value;

        public AttributeValue Value
        {
            get => _value;
            set => _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Product Product { get; set; } = default!;
        public ProductAttribute Attribute { get; set; } = default!;
    }

}
