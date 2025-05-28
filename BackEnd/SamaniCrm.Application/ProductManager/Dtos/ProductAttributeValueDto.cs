using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductAttributeValueDto
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
