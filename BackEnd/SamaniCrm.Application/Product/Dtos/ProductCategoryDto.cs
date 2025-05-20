using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Application.Product.Dtos
{
    public class ProductCategoryDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Slug { get; set; }
        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }


        public Guid? ParentId { get; set; }

        public MenuTargetEnum Target { get; set; } = MenuTargetEnum.Self;

        public List<ProductCategoryDto> Children { get; set; } = [];

        public List<ProductCategoryTranslationDto>? Translations { get; set; }
        public string Description { get; set; }
    }

    public class ProductCategoryTranslationDto
    {
        public Guid ProductCategoryId { get; set; }
        public required string Culture { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }


}
