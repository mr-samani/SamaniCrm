using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedCurrencies
    {
        public static async Task TrySeedAsync(TenantDbContext dbContext, Guid? tenantId)
        {
            Console.WriteLine("Seeding static curerncies data...");

            var IRR = new Currency()
            {
                TenantId = tenantId,
                CurrencyCode = "IRR",
                Name = "Iranian Rial",
                ExchangeRateToBase = 1,
                IsActive = true,
                IsDefault = true,
                Symbol = "﷼",
            };
            var TMN = new Currency()
            {
                TenantId = tenantId,
                CurrencyCode = "TMN",
                Name = "Iranian Toman",
                ExchangeRateToBase = (decimal)0.10,
                IsActive = true,
                IsDefault = false,
                Symbol = "تومان",
            };
            var USD = new Currency()
            {
                TenantId = tenantId,
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
