using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedCurrencies
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Seeding static curerncies data...");

            var IRR = new Currency()
            {
                CurrencyCode = "IRR",
                Name = "Iranian Rial",
                ExchangeRateToBase = 1,
                IsActive = true,
                IsDefault = true,
                Symbol = "﷼",
            };
            var TMN = new Currency()
            {
                CurrencyCode = "TMN",
                Name = "Iranian Toman",
                ExchangeRateToBase = (decimal)0.10,
                IsActive = true,
                IsDefault = false,
                Symbol = "تومان",
            };
            var USD = new Currency()
            {
                CurrencyCode = "USD",
                Name = "US dolar",
                ExchangeRateToBase = 800000,
                IsActive = true,
                IsDefault = false,
                Symbol = "$",
            };


            var existingCurrency = await dbContext.Currency.IgnoreQueryFilters().ToListAsync();
            if (!existingCurrency.Any())
            {
                await dbContext.Currency.AddRangeAsync(IRR, TMN, USD);
                await dbContext.SaveChangesAsync();
            }


            Console.WriteLine("seed currenies ended");
        }
    }
}
