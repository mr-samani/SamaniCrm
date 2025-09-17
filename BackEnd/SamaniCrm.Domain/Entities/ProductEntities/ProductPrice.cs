using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductPrice : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        [MaxLength(5)]
        public string CurrencyCode { get; set; } = default!;  // مثلا "USD", "IRR"
        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PriceTypeEnum Type { get; set; }

        public virtual Product Product { get; set; } = default!;
        public virtual Currency Currency { get; set; } = default!;


        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

        // Implementing ISoftDelete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? DeletedBy { get; set; }
    }


  

}
