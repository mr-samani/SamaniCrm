using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedLocalization
    {
        public static async Task TrySeedAsync(MasterDbContext masterDbContext,TenantDbContext tenantDbContext,Guid? tenantId)
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
            var existingLanguages = await masterDbContext.Languages.ToListAsync();
            if (!existingLanguages.Any())
            {
                await masterDbContext.Languages.AddRangeAsync(languages);
                await masterDbContext.SaveChangesAsync();
            }


            //-----------------------------------------------------
            var newLocalizations = new List<Localization>();
            var allLanguages = masterDbContext.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Culture)
                .ToList();


            // seed roles
            var roles = tenantDbContext.Roles
                .Where(x=>x.TenantId == tenantId)
                .Select(s => "Role:" + s.Name)
                .Distinct()
                .ToList();
            var existingRoleLocalizations = masterDbContext.Localizations
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
            var allPermissions = tenantDbContext.Permissions
                .Select(p => p.LocalizeKey)
                .Distinct()
                .ToList();


            var existingPermissionLocalizations = masterDbContext.Localizations
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
                masterDbContext.Localizations.AddRange(newLocalizations);
                masterDbContext.SaveChanges();
            }

        }
    }
}
