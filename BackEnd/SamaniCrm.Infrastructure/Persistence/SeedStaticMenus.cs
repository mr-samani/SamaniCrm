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
                    .FirstOrDefaultAsync(m => m.Code == menu.Code && m.ParentId == null);

                if (existingMenu == null)
                {
                    await dbContext.Menus.AddAsync(menu);
                }
                else if (menu.Children?.Any() == true)
                {
                    foreach (var child in menu.Children)
                    {
                        var existingChild = existingMenu.Children
                            .FirstOrDefault(c => c.Code == child.Code);

                        if (existingChild == null)
                        {
                            existingMenu.Children.Add(child);
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private static List<Menu> GetStaticMenus()
        {
            return new List<Menu>
            {
                new Menu
                {
                    Code = "Home",
                    Icon = "fa fa-home",
                    Url = "/",
                    OrderIndex = 0,
                },
                new Menu
                {
                    Code = "AboutUs",
                    Icon = "fa fa-home",
                    Url = "/about-us",
                    OrderIndex = 2,
                },
                new Menu
                {
                    Code = "ContactUs",
                    Icon = "fa fa-home",
                    Url = "/contact-us",
                    OrderIndex = 1,
                },
                new Menu
                {
                    Code = "Blogs",
                    Icon = "fa fa-home",
                    Url = "/blogs",
                    OrderIndex = 3,
                    Children = new List<Menu>
                    {
                        new Menu { Code = "BlogTest1", Url = "/blogs/1" },
                        new Menu { Code = "BlogTest2", Url = "/blogs/2" },
                        new Menu { Code = "BlogTest3", Url = "/blogs/3" }
                    }
                }
            };
        }
    }
}
