using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class ProductPriceDto
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PriceTypeEnum Type { get; set; }
    }
}
