using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedLocalization
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Try seed localization data");
            var languages = new List<Language>
            {
                new Language
                {
                    Name = "فارسی (Persian)",
                    Culture = "fa-IR",
                    IsDefault = true,
                    IsRtl = true
                },
                new Language
                {
                    Name = "English",
                    Culture = "en-US",
                    IsDefault = false,
                    IsRtl = false
                }
            };
            var existingLanguages = await dbContext.Languages.ToListAsync();
            if (!existingLanguages.Any())
            {
                await dbContext.Languages.AddRangeAsync(languages);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
