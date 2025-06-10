using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductFileDto
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public Guid FileId { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
