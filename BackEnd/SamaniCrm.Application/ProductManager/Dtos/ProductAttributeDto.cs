using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductAttributeDto
    {
        public Guid? Id { get; set; }
        public Guid ProductTypeId { get; set; }
        public string DataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsVariant { get; set; }
        public int SortOrder { get; set; }
        public List<ProductAttributeTranslationDto>? Translations { get; set; }
    }

    public class ProductAttributeTranslationDto
    {
        public Guid ProductAttributeId { get; set; }
        public string Culture { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
