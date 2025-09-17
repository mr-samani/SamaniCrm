﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeDetective.Storage;
using Newtonsoft.Json.Linq;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedEnums
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Seeding enums...");

            var enumTypes = new List<Type> {
                typeof(ProductAttributeDataTypeEnum),
                typeof(LocalizationCategoryEnum) ,
                typeof(MenuTargetEnum),
                typeof(NotificationPeriorityEnum),
                typeof(NotificationTypeEnum),
                typeof(PageStatusEnum),
                typeof(PageTypeEnum),
                typeof(DiscountTypeEnum),
                typeof(PriceTypeEnum),
                typeof(TwoFactorTypeEnum),
                typeof(CacheProvider)

            };
            var seedLocalizations = new List<Localization>();

            foreach (var enumType in enumTypes)
            {
                var enumValues = Enum.GetValues(enumType).Cast<Enum>();
                foreach (var enumValue in enumValues)
                {
                    // کلید: EnumTypeName_Value
                    string key = $"{enumType.Name}_{Convert.ToInt32(enumValue)}";

                    // مقدار: اگر attribute داشت استفاده کن، وگرنه اسم enum
                    string value = enumValue.ToString();

                    // اگر میخوای از attribute مثل DisplayName استفاده کنی:
                    var displayAttr = enumValue.GetType()
                        .GetField(enumValue.ToString())
                        .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                        .FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;

                    if (displayAttr != null)
                    {
                        value = displayAttr.Name; // یا ResourceName
                    }

                    seedLocalizations.Add(new Localization
                    {
                        Culture = "fa-IR",
                        Key = key,
                        Value = value,
                        Category = LocalizationCategoryEnum.Enum
                    });
                }
            }



            // دریافت همه کلیدهای موجود با Culture مورد نظر
            var existingKeys = await dbContext.Localizations
                .Where(x => x.Culture == "fa-IR")
                .Select(x => x.Key)
                .ToListAsync();

            // فقط مواردی که وجود ندارند را اضافه می‌کنیم
            var toAdd = seedLocalizations
                .Where(x => !existingKeys.Contains(x.Key))
                .ToList();

            if (toAdd.Any())
            {
                await dbContext.Localizations.AddRangeAsync(toAdd);
                await dbContext.SaveChangesAsync();
            }


            Console.WriteLine("seed enums ended");
        }
    }
}
