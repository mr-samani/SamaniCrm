using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedStaticMenus
    {
        public static async Task TrySeedAsync(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Seeding static menu data...");

            var staticMenus = GetStaticMenus();

            foreach (var menu in staticMenus)
            {
                var existingMenu = await dbContext.Menus
                    .Include(m => m.Children)
                    .FirstOrDefaultAsync(m => m.ParentId == null);

                if (existingMenu == null)
                {
                    await dbContext.Menus.AddAsync(menu);
                }
                else if (menu.Children?.Any() == true)
                {
                    foreach (var child in menu.Children)
                    {
                        var existingChild = existingMenu.Children
                            .FirstOrDefault();

                        if (existingChild == null)
                        {
                            existingMenu.Children.Add(child);
                        }
                    }
                }
            }

            // seed menus translations
            List<MenuTranslation> newTranslations = [];
            var allLanguages = await dbContext.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Culture)
                .ToListAsync();
            var allMenus = await dbContext.Menus.Select(s => new { s.Id, s.Url }).ToListAsync();
            var existingTranslations = await dbContext.MenuTranslations
              .Select(l => new { l.Id, l.MenuId, l.Culture })
              .ToListAsync();
            foreach (var mnu in allMenus)
            {
                foreach (var culture in allLanguages)
                {
                    bool translationExists = existingTranslations.Any(et =>
                                                        et.MenuId == mnu.Id && et.Culture == culture);
                    if (!translationExists)
                    {
                        newTranslations.Add(new MenuTranslation
                        {
                            MenuId = mnu.Id,
                            Culture = culture,
                            Title = mnu.Url ?? "",
                        });
                    }
                }
            }
            if (newTranslations.Any())
            {
                await dbContext.MenuTranslations.AddRangeAsync(newTranslations);
            }


            await dbContext.SaveChangesAsync();
        }

        private static List<Menu> GetStaticMenus()
        {
            return new List<Menu>
            {
                new Menu
                {
                    Icon = "fa fa-home",
                    Url = "/",
                    OrderIndex = 0,
                    IsSystem = true,
                },
                new Menu
                {
                    Icon = "fa fa-company",
                    Url = "/about-us",
                    OrderIndex = 2,
                    IsSystem = true,
                },
                new Menu
                {
                    Icon = "fa fa-envelope",
                    Url = "/contact-us",
                    OrderIndex = 1,
                    IsSystem = true,
                },
                new Menu
                {
                    Icon = "fa fa-book",
                    Url = "/blogs",
                    OrderIndex = 3,
                    Children = new List<Menu>
                    {
                        new Menu { Url = "/blogs/1" },
                        new Menu { Url = "/blogs/2" },
                        new Menu { Url = "/blogs/3" }
                    }
                }
            };
        }
    }
}
