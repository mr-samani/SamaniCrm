using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Enums;
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
                    Flag = "fa-IR",
                    IsDefault = true,
                    IsRtl = true
                },
                new Language
                {
                    Name = "English",
                    Culture = "en-US",
                    Flag = "en-US",
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
            var newLocalizations = new List<Localization>();
            var allLanguages = dbContext.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Culture)
                .ToList();


            // seed roles
            var roles = dbContext.Roles.Select(s => "Role:" + s.Name).Distinct().ToList();
            var existingRoleLocalizations = dbContext.Localizations
                .Where(l => roles.Contains(l.Key))
                .Select(l => new { l.Key, l.Culture })
                .IgnoreQueryFilters()
                .ToHashSet();
            foreach (var roleKey in roles)
            {
                foreach (var culture in allLanguages)
                {
                    if (!existingRoleLocalizations.Contains(new { Key = roleKey, Culture = culture }))
                    {
                        newLocalizations.Add(new Localization
                        {
                            Key = roleKey,
                            Culture = culture,
                            Value = string.Empty, // بعداً توسط کاربر تکمیل می شود
                            Category = LocalizationCategoryEnum.Role,
                        });
                    }
                }
            }



            // seed permission to localization 
            var allPermissions = dbContext.Permissions
                .Select(p => p.LocalizeKey)
                .Distinct()
                .ToList();


            var existingPermissionLocalizations = dbContext.Localizations
                .Where(l => allPermissions.Contains(l.Key))
                .Select(l => new { l.Key, l.Culture })
                .ToHashSet();
            foreach (var permissionLocalizeKey in allPermissions)
            {
                foreach (var culture in allLanguages)
                {
                    if (!existingPermissionLocalizations.Contains(new { Key = permissionLocalizeKey, Culture = culture }))
                    {
                        newLocalizations.Add(new Localization
                        {
                            Key = permissionLocalizeKey,
                            Culture = culture,
                            Value = string.Empty, // بعداً توسط کاربر تکمیل می شود
                            Category= LocalizationCategoryEnum.Permission,
                        });
                    }
                }
            }



            // seed localize enums
            // newLocalizations.Concat()



          

            Console.WriteLine("new localize count:" + newLocalizations.Count);
            if (newLocalizations.Any())
            {
                dbContext.Localizations.AddRange(newLocalizations);
                dbContext.SaveChanges();
            }

        }
    }
}
