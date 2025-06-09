using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductImageDto
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public Guid FileId { get; set; } 
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }
}
