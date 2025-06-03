using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductInfoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? CategoryTitle { get; set; }
        public string? ProductTypeTitle { get; set; }
        public string Content { get; set; } = string.Empty;

        public string? Tags { get; set; }

        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? CreationTime { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<ProductFileDto> Files { get; set; } = new List<ProductFileDto>();
        public List<ProductPriceDto> Prices { get; set; } = new List<ProductPriceDto>();
        public List<ProductAttributeInfoDto> AttributeValues { get; set; } = new List<ProductAttributeInfoDto>();
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

    public class ProductAttributeInfoDto
    {
        public Guid AttributeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public ProductAttributeDataTypeEnum DataType { get; set; }
    }
}
