using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductFileDto
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}
