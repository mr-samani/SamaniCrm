using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductDto
    {
        public Guid? Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? CategoryTitle { get; set; }
        public string? ProductTypeTitle { get; set; }   



        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? CreationTime { get; set; }
        public List<ProductTranslationDto>? Translations { get; set; }
        public List<ProductImageDto>? Images { get; set; }
        public List<ProductFileDto>? Files { get; set; }
        public List<ProductPriceDto>? Prices { get; set; }
        public List<ProductAttributeValueDto>? AttributeValues { get; set; }
    }

    public class ProductTranslationDto
    {
        public Guid? ProductId { get; set; }
        public string Culture { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    //public class ProductImageDto
    //{
    //    public Guid? Id { get; set; }
    //    public string Url { get; set; } = string.Empty;
    //    public bool IsMain { get; set; }
    //    public int SortOrder { get; set; }
    //}

    //public class ProductFileDto
    //{
    //    public Guid? Id { get; set; }
    //    public string FileUrl { get; set; } = string.Empty;
    //    public string FileType { get; set; } = string.Empty;
    //}

    //public class ProductPriceDto
    //{
    //    public Guid? Id { get; set; }
    //    public string Currency { get; set; } = string.Empty;
    //    public decimal Price { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime? EndDate { get; set; }
    //}

    //public class ProductAttributeValueDto
    //{
    //    public Guid? Id { get; set; }
    //    public Guid AttributeId { get; set; }
    //    public string Value { get; set; } = string.Empty;
    //}
}
