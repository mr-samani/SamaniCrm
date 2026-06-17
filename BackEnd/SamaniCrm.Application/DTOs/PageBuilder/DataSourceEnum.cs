using SamaniCrm.Application.ProductManagerManager.Dtos;
using System.ComponentModel.DataAnnotations;


namespace SamaniCrm.Application.DTOs.PageBuilder;

public enum DataSourceEnum
{
    ProductCategories = 1,
    Products
}


public class DynamicDataProductCategoriesFields
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Slug { get; set; }
    public int OrderIndex { get; set; }

}

public class DynamicDataProductsFields
{
    public Guid? Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }

    public string? CategoryTitle { get; set; }
    public string? ProductTypeTitle { get; set; }

    public string? Tags { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<ProductImageDto>? Images { get; set; }
    public List<ProductFileDto>? Files { get; set; }
    public List<ProductPriceDto>? Prices { get; set; }
    public List<ProductAttributeValueDto>? AttributeValues { get; set; }
}