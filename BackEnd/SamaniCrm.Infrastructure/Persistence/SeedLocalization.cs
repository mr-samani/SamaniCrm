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


            //-----------------------------------------------------
            // seed permission to localization 
            var allPermissions = dbContext.Permissions
        .Select(p => p.LocalizeKey)
        .Distinct()
        .ToList();

            var allLanguages = dbContext.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Culture)
                .ToList();

            var existingLocalizations = dbContext.Localizations
                .Where(l => allPermissions.Contains(l.Key))
                .Select(l => new { l.Key, l.Culture })
                .ToHashSet();

            var newLocalizations = new List<Localization>();

            foreach (var permissionKey in allPermissions)
            {
                foreach (var culture in allLanguages)
                {
                    if (!existingLocalizations.Contains(new { Key = permissionKey, Culture = culture }))
                    {
                        newLocalizations.Add(new Localization
                        {
                            Key = permissionKey,
                            Culture = culture,
                            Value = string.Empty, // بعداً توسط کاربر تکمیل می‌شود
                        });
                    }
                }
            }
            Console.WriteLine("new localize count:",newLocalizations.Count);
            if (newLocalizations.Any())
            {
                dbContext.Localizations.AddRange(newLocalizations);
                dbContext.SaveChanges();
            }

        }
    }
}
