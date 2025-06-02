using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductListDto
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
    }

}
