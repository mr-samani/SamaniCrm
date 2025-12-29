using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductTypeDto
    {
        public Guid? Id { get; set; }
        public List<ProductAttributeDto>? Attributes { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreationTime { get; set; }

        public List<ProductTypeTranslationDto>? Translations { get; set; }
    }

    public class ProductTypeTranslationDto
    {
        public Guid? ProductTypeId { get; set; }
        public required string Culture { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

}
