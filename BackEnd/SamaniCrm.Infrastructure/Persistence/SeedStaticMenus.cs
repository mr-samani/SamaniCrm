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
                var existingMenus = await dbContext.Menus
                    .Include(m => m.Children) // برای اینکه بتوانیم Children ها را هم بررسی کنیم
                    .Include(m => m.Translations) // برای اینکه بتوانیم Translations ها را هم بررسی کنیم
                    .ToListAsync();

                var menusToAdd = new List<Menu>();
                var translationsToAdd = new List<MenuTranslation>();

                // دریافت تمام زبان‌های فعال
                var activeLanguages = await dbContext.Languages
                    .Where(l => l.IsActive)
                    .Select(l => l.Culture)
                    .ToListAsync();

            // ساخت یک دیکشنری برای دسترسی سریع به منوهای موجود بر اساس URL (یا یک شناسه منحصربفرد دیگر)
            // اگر URL ها در دیتابیس منحصربفرد نیستند، باید از ترکیب ParentId و URL یا چیز دیگری استفاده کرد.
            Dictionary<string, Menu>? existingMenusByURL = existingMenus?.ToDictionary(m => m.Url??"x", m => m);

                // پردازش منوهای استاتیک و ساخت لیست منوهایی که باید اضافه شوند
                foreach (var staticMenu in staticMenus)
                {
                    Menu? existingMenu = null;
                    if (existingMenusByURL!= null &&  existingMenusByURL.TryGetValue(staticMenu.Url??"x", out existingMenu))
                    {
                        // منو وجود دارد، حالا ترجمه‌ها و فرزندان آن را بررسی می‌کنیم
                        await UpdateMenuAndChildren(dbContext, existingMenu, staticMenu, activeLanguages, translationsToAdd);
                    }
                    else
                    {
                        // منو وجود ندارد، آن را برای اضافه کردن علامت‌گذاری می‌کنیم
                        // ابتدا ترجمه‌های آن را اضافه می‌کنیم
                        foreach (var lang in activeLanguages)
                        {
                            var staticMenuTranslation = staticMenu.Translations.FirstOrDefault(t => t.Culture == lang);
                            if (staticMenuTranslation != null)
                            {
                                translationsToAdd.Add(new MenuTranslation
                                {
                                    Culture = lang,
                                    Title = staticMenuTranslation.Title,
                                });
                            }
                            else
                            {
                                // اگر برای زبان فعال ترجمه ای در منوی استاتیک نبود، یک ترجمه پیش فرض اضافه کنید
                                translationsToAdd.Add(new MenuTranslation
                                {
                                    Culture = lang,
                                    Title = staticMenu.Url ?? $"Menu {staticMenu.OrderIndex}", // یک عنوان پیش فرض
                                });
                            }
                        }

                        // اگر منو فرزند دارد، آن‌ها را نیز به صورت بازگشتی پردازش می‌کنیم
                        if (staticMenu.Children != null && staticMenu.Children.Any())
                        {
                            staticMenu.Children = await ProcessChildren(staticMenu.Children, activeLanguages, translationsToAdd, dbContext);
                        }

                        menusToAdd.Add(staticMenu);
                    }
                }

                // اضافه کردن منوهای جدید
                if (menusToAdd.Any())
                {
                    await dbContext.Menus.AddRangeAsync(menusToAdd);
                    // SaveChangesAsync را در انتها صدا می‌زنیم
                }

                // اضافه کردن ترجمه‌های جدید (که قبلا جمع‌آوری شدند)
                if (translationsToAdd.Any())
                {
                    // فیلتر کردن ترجمه‌های تکراری که ممکن است در پردازش منوهای موجود ایجاد شده باشند
                    var existingTranslationKeys = await dbContext.MenuTranslations
                        .Select(x => $"{x.MenuId}_{x.Culture}")
                        .ToHashSetAsync();

                    var uniqueTranslationsToAdd = translationsToAdd
                        .Where(t => !existingTranslationKeys.Contains($"{t.MenuId}_{t.Culture}"))
                        .ToList();

                    if (uniqueTranslationsToAdd.Any())
                    {
                        await dbContext.MenuTranslations.AddRangeAsync(uniqueTranslationsToAdd);
                        // SaveChangesAsync را در انتها صدا می‌زنیم
                    }
                }

                // تنها یک بار SaveChangesAsync را در انتها فراخوانی کنید
                if (dbContext.ChangeTracker.HasChanges())
                {
                    await dbContext.SaveChangesAsync(); // ارسال فلگ برای نادیده گرفتن AuditLog
                }

                Console.WriteLine("Static menu data seeding completed.");
            }

            // تابع کمکی برای پردازش منوهای موجود و فرزندان آنها
            private static async Task UpdateMenuAndChildren(
                ApplicationDbContext dbContext,
                Menu existingMenu,
                Menu staticMenu,
                List<string> activeLanguages,
                List<MenuTranslation> translationsToAdd
           )
            {
                // به‌روزرسانی ویژگی‌های منوی موجود (Icon, OrderIndex, IsSystem)
                existingMenu.Icon = staticMenu.Icon ?? existingMenu.Icon;
                existingMenu.OrderIndex = staticMenu.OrderIndex;
                existingMenu.IsSystem = staticMenu.IsSystem;
                // اگر URL تغییر کرده بود، اینجا هم آپدیت کنید
                // existingMenu.Url = staticMenu.Url ?? existingMenu.Url;

                // پردازش ترجمه‌های منوی موجود
                foreach (var lang in activeLanguages)
                {
                    var existingTranslation = existingMenu.Translations.FirstOrDefault(t => t.Culture == lang);
                    var staticMenuTranslation = staticMenu.Translations.FirstOrDefault(t => t.Culture == lang);

                    if (staticMenuTranslation != null)
                    {
                        if (existingTranslation == null)
                        {
                            // ترجمه جدید برای این زبان اضافه کن
                            translationsToAdd.Add(new MenuTranslation
                            {
                                MenuId = existingMenu.Id,
                                Culture = lang,
                                Title = staticMenuTranslation.Title,
                            });
                        }
                        else
                        {
                            // ترجمه موجود را به‌روزرسانی کن
                            existingTranslation.Title = staticMenuTranslation.Title;
                        }
                    }
                    // اگر ترجمه ای در منوی استاتیک برای این زبان نبود، کاری انجام نده یا ترجمه موجود را نگه دار
                }

                // پردازش فرزندان (به صورت بازگشتی)
                if (staticMenu.Children != null && staticMenu.Children.Any())
                {
                    staticMenu.Children = await ProcessChildren(staticMenu.Children, activeLanguages, translationsToAdd, dbContext);
                    // اضافه کردن فرزندان جدید به منوی موجود
                    foreach (var newChild in staticMenu.Children.Where(c => !existingMenu.Children.Any(ec => ec.Url == c.Url)))
                    {
                        existingMenu.Children.Add(newChild);
                    }
                }
            }

            // تابع کمکی برای پردازش لیست فرزندان
            private static async Task<List<Menu>> ProcessChildren(
                ICollection<Menu> children,
                List<string> activeLanguages,
                List<MenuTranslation> translationsToAdd,
                ApplicationDbContext dbContext)
            {
                var processedChildren = new List<Menu>();
                var existingChildren = await dbContext.Menus
                    .Where(m => m.ParentId != null && children.Select(c => c.Url).Contains(m.Url))
                    .Include(m => m.Translations)
                    .ToListAsync();

                var existingChildrenByURL = existingChildren.ToDictionary(m => m.Url??"x", m => m);

                foreach (var child in children)
                {
                    Menu? existingChild = null;
                    if (existingChildrenByURL.TryGetValue(child.Url??"x", out existingChild))
                    {
                        // فرزند موجود است، آن را به‌روزرسانی کن
                        await UpdateMenuAndChildren(dbContext, existingChild, child, activeLanguages, translationsToAdd);
                        processedChildren.Add(existingChild);
                    }
                    else
                    {
                        // فرزند جدید است، آن را برای اضافه کردن آماده کن
                        // اضافه کردن ترجمه‌ها برای فرزند جدید
                        foreach (var lang in activeLanguages)
                        {
                            var staticMenuTranslation = child.Translations.FirstOrDefault(t => t.Culture == lang);
                            if (staticMenuTranslation != null)
                            {
                                translationsToAdd.Add(new MenuTranslation
                                {
                                    Culture = lang,
                                    Title = staticMenuTranslation.Title,
                                });
                            }
                            else
                            {
                                translationsToAdd.Add(new MenuTranslation
                                {
                                    Culture = lang,
                                    Title = child.Url ?? $"Sub Menu {child.OrderIndex}",
                                });
                            }
                        }
                        // اگر فرزندان بیشتری دارد، اینجا به صورت بازگشتی پردازش کن
                        if (child.Children != null && child.Children.Any())
                        {
                            child.Children = await ProcessChildren(child.Children, activeLanguages, translationsToAdd, dbContext);
                        }
                        processedChildren.Add(child);
                    }
                }
                return processedChildren;
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
                    Translations=[
                            new MenuTranslation(){
                                Culture = "en-US",
                                Title = "Home",
                            },
                              new MenuTranslation(){
                                Culture = "fa-IR",
                                Title = "صفحه اصلی",
                            },
                        ]
                },
                new Menu
                {
                    Icon = "fa fa-package",
                    Url = "/products",
                    OrderIndex = 0,
                    IsSystem = true,
                    Translations=[
                            new MenuTranslation(){
                                Culture = "en-US",
                                Title = "Products",
                            },
                              new MenuTranslation(){
                                Culture = "fa-IR",
                                Title = "محصولات",
                            },
                        ]
                },
                new Menu
                {
                    Icon = "fa fa-company",
                    Url = "/about-us",
                    OrderIndex = 2,
                    IsSystem = true,
                    Translations=[
                            new MenuTranslation(){
                                Culture = "en-US",
                                Title = "About us",
                            },
                              new MenuTranslation(){
                                Culture = "fa-IR",
                                Title = "درباره ما",
                            },
                        ]
                },
                new Menu
                {
                    Icon = "fa fa-envelope",
                    Url = "/contact-us",
                    OrderIndex = 1,
                    IsSystem = true,
                    Translations=[
                            new MenuTranslation(){
                                Culture = "en-US",
                                Title = "Contact us",
                            },
                              new MenuTranslation(){
                                Culture = "fa-IR",
                                Title = "تماس با ما",
                            },
                        ]
                },
                new Menu
                {
                    Icon = "fa fa-book",
                    Url = "/blogs",
                    OrderIndex = 3,
                    Translations=[
                            new MenuTranslation(){
                                Culture = "en-US",
                                Title = "Blog",
                            },
                              new MenuTranslation(){
                                Culture = "fa-IR",
                                Title = "بلاگ",
                            },
                        ],
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
