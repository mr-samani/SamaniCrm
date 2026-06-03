using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;

namespace SamaniCrm.Infrastructure.Persistence
{
    public static class SeedStaticMenus
    {
        public static async Task TrySeedAsync(MasterDbContext masterDbContext, TenantDbContext dbContext)
        {
            Console.WriteLine("Seeding static menu data...");

            List<string> activeLanguages = await masterDbContext.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Culture)
                .ToListAsync();

            List<Menu> existingMenus = await dbContext.Menus
                .AsSplitQuery()
                .Include(m => m.Translations)
                .Include(m => m.Children)
                .ToListAsync();


            Dictionary<string, Menu> existingMenusByName = existingMenus.ToDictionary(m => m.Name, m => m);

            // 3. پردازش منوهای استاتیک
            var staticMenus = GetStaticMenus();
            await ProcessMenusAsync(staticMenus, null, dbContext, activeLanguages, existingMenusByName);

            // 4. ذخیره نهایی
            if (dbContext.ChangeTracker.HasChanges())
            {
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Static menu data seeding completed.");
        }

        /// <summary>
        /// پردازش بازگشتی منوها
        /// </summary>
        private static async Task ProcessMenusAsync(
            IEnumerable<Menu> staticMenus,
            Menu? parentMenu,
            TenantDbContext dbContext,
            List<string> activeLanguages,
            Dictionary<string, Menu> existingMenusByName)
        {
            foreach (var staticMenu in staticMenus)
            {
                // تنظیم ParentId
                staticMenu.ParentId = parentMenu?.Id;

                // بررسی وجود منو
                if (existingMenusByName.TryGetValue(staticMenu.Name, out var existingMenu))
                {
                    // منو وجود دارد -> Update
                    UpdateExistingMenu(existingMenu, staticMenu, activeLanguages);
                }
                else
                {
                    // منو وجود ندارد -> Insert
                    // ترجمه‌های پیش‌فرض را برای زبان‌هایی که ترجمه ندارند اضافه کن
                    EnsureDefaultTranslations(staticMenu, activeLanguages);

                    await dbContext.Menus.AddAsync(staticMenu);

                    // این منوی جدید را در دیکشنری اضافه کن تا فرزندان بتوانند به آن ارجاع دهند
                    // نکته: این کار فقط برای مدیریت رابطه در حافظه است، در دیتابیس هنوز Save نشده
                    // اما چون در همان تراکنش SaveChanges انجام می‌شود، EF Core مدیریت می‌کند
                }

                // پردازش فرزندان
                if (staticMenu.Children != null && staticMenu.Children.Any())
                {
                    // برای فرزندان، باید دیکشنری را آپدیت کنیم تا منوی فعلی (چه جدید چه موجود) را پیدا کنند
                    // اما چون EF Core رابطه را بر اساس Navigation Property مدیریت می‌کند، 
                    // کافیست parentMenu را به فرزند بدهیم.

                    // نکته مهم: اگر منوی جدید است، Id آن هنوز ندارد. 
                    // اما وقتی آن را به dbContext.AddAsync می‌دهیم، EF Core آن را در ChangeTracker نگه می‌دارد.
                    // وقتی فرزندان را هم اضافه کنیم، EF Core به ترتیب صحیح (پدر قبل از فرزند) Save می‌کند.

                    await ProcessMenusAsync(
                        staticMenu.Children,
                        existingMenu ?? staticMenu, // اگر منوی جدید است، خود staticMenu را به عنوان پدر بدهیم
                        dbContext,
                        activeLanguages,
                        existingMenusByName);
                }
            }
        }

        private static void UpdateExistingMenu(Menu existingMenu, Menu staticMenu, List<string> activeLanguages)
        {
            // آپدیت فیلدهای ساده
            existingMenu.Icon = staticMenu.Icon ?? existingMenu.Icon;
            existingMenu.OrderIndex = staticMenu.OrderIndex;
            existingMenu.IsSystem = staticMenu.IsSystem;
            existingMenu.Name = staticMenu.Name;

            // آپدیت ترجمه‌ها
            foreach (var lang in activeLanguages)
            {
                var existingTranslation = existingMenu.Translations.FirstOrDefault(t => t.Culture == lang);
                var staticTranslation = staticMenu.Translations.FirstOrDefault(t => t.Culture == lang);

                if (staticTranslation != null)
                {
                    if (existingTranslation == null)
                    {
                        // ترجمه جدید اضافه کن
                        existingMenu.Translations.Add(new MenuTranslation
                        {
                            MenuId = existingMenu.Id, // Id از دیتابیس خوانده شده است
                            Culture = lang,
                            Title = staticTranslation.Title
                        });
                    }
                    else
                    {
                        // ترجمه موجود را آپدیت کن
                        existingTranslation.Title = staticTranslation.Title;
                    }
                }
            }

            // آپدیت فرزندان
            if (staticMenu.Children != null && staticMenu.Children.Any())
            {
                // حذف فرزندان قدیمی که در لیست جدید نیستند (اختیاری، بسته به نیاز)
                // اضافه کردن فرزندان جدید
                var existingChildNames = existingMenu.Children.Select(c => c.Name).ToHashSet();
                foreach (var staticChild in staticMenu.Children)
                {
                    if (!existingChildNames.Contains(staticChild.Name))
                    {
                        // فرزند جدید است
                        staticChild.ParentId = existingMenu.Id;
                        EnsureDefaultTranslations(staticChild, activeLanguages);
                        existingMenu.Children.Add(staticChild);
                    }
                    else
                    {
                        // فرزند موجود است، باید آن را آپدیت کنیم
                        var existingChild = existingMenu.Children.First(c => c.Name == staticChild.Name);
                        UpdateExistingMenu(existingChild, staticChild, activeLanguages);
                    }
                }
            }
        }

        private static void EnsureDefaultTranslations(Menu menu, List<string> activeLanguages)
        {
            if (menu.Translations == null)
            {
                menu.Translations = new List<MenuTranslation>();
            }

            foreach (var lang in activeLanguages)
            {
                if (!menu.Translations.Any(t => t.Culture == lang))
                {
                    // اگر ترجمه‌ای برای این زبان وجود ندارد، یک ترجمه پیش‌فرض اضافه کن
                    menu.Translations.Add(new MenuTranslation
                    {
                        Culture = lang,
                        Title = menu.Name,
                        MenuId = Guid.NewGuid()
                    });
                }
            }
        }

        private static List<Menu> GetStaticMenus()
        {
            return new List<Menu>
            {
                new Menu
                {
                    Name="Home",
                    Icon = "fa fa-home",
                    Url = "/",
                    OrderIndex = 0,
                    IsSystem = true,
                    Translations = new List<MenuTranslation>
                    {
                        new MenuTranslation { Culture = "en-US", Title = "Home" },
                        new MenuTranslation { Culture = "fa-IR", Title = "صفحه اصلی" }
                    }
                },
                new Menu
                {
                    Name="Products",
                    Icon = "fa fa-package",
                    Url = "/products",
                    OrderIndex = 1,
                    IsSystem = true,
                    Translations = new List<MenuTranslation>
                    {
                        new MenuTranslation { Culture = "en-US", Title = "Products" },
                        new MenuTranslation { Culture = "fa-IR", Title = "محصولات" }
                    }
                },
                new Menu
                {
                    Name="Blogs",
                    Icon = "fa fa-book",
                    Url = "/blogs",
                    OrderIndex = 2,
                    IsSystem = true,
                    Translations = new List<MenuTranslation>
                    {
                        new MenuTranslation { Culture = "en-US", Title = "Blog" },
                        new MenuTranslation { Culture = "fa-IR", Title = "بلاگ" }
                    },
                    Children = new List<Menu>
                    {
                        new Menu
                        {
                            Name="Blogs-1",
                            Url = "/blogs/1",
                            Translations = new List<MenuTranslation>
                            {
                                new MenuTranslation { Culture = "en-US", Title = "Blog Post 1" },
                                new MenuTranslation { Culture = "fa-IR", Title = "نوشته بلاگ ۱" }
                            }
                        },
                        new Menu
                        {
                            Name="Blocgs-2",
                            Url = "/blogs/2",
                            Translations = new List<MenuTranslation>
                            {
                                new MenuTranslation { Culture = "en-US", Title = "Blog Post 2" },
                                new MenuTranslation { Culture = "fa-IR", Title = "نوشته بلاگ ۲" }
                            }
                        }
                    }
                }
            };
        }
    }
}