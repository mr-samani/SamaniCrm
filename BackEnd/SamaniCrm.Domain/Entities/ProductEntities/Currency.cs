using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class Currency : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        [MaxLength(5)]
        public required string CurrencyCode { get; set; }
        [MaxLength(100)]    
        public required string Name { get; set; }
        [MaxLength(100)]
        public string Symbol { get; set; } = string.Empty; // $ , ريال

        [Description("rate for convert base to this")]
        public decimal ExchangeRateToBase { get; set; }
        public virtual Collection<ProductPrice>? ProductPrices { get; set; }

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }



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
