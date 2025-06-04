using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Dtos
{
    public class CurrencyDto
    {
        public Guid? Id { get; set; }
        [MaxLength(5)]
        public required string CurrencyCode { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(100)]
        public string Symbol { get; set; } = string.Empty; // $ , ريال

        [Description("rate for convert base to this")]
        public decimal ExchangeRateToBase { get; set; }

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
