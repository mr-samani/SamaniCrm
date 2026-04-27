using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence;

public static class SeedPages
{
    public static async Task TrySeedAsync(ApplicationDbContext dbContext)
    {



        var productPages = await dbContext.Pages
                       .Where(p => p.Type == PageTypeEnum.Product).ToListAsync();


        Console.WriteLine("product page count:" + productPages.Count);

        if (!productPages.Any())
        {

            var list = new List<Page>();
            list.Add(new Page()
            {
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                    new PageTranslation(){
                        Culture = "fa-IR",
                        Title = "دسته بندی محصولات",
                        Description = "",
                    },
                    new PageTranslation(){
                        Culture = "en-US",
                        Title = "Category of products",
                        Description = "",
                    },
                    ]
            });

            list.Add(new Page()
            {
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                new PageTranslation(){
                        Culture = "fa-IR",
                        Title = "محصولات",
                        Description = "",
                    },
                    new PageTranslation(){
                        Culture = "en-US",
                        Title = "Products",
                        Description = "",
                    },
                    ]
            });


            list.Add(new Page()
            {
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                new PageTranslation(){
                        Culture = "fa-IR",
                        Title = "سبد خرید",
                        Description = "",
                    },
                    new PageTranslation(){
                        Culture = "en-US",
                        Title = "Cart",
                        Description = "",
                    },
                    ]
            });


            Console.WriteLine("new page count:" + list.Count);
            dbContext.Pages.AddRange(list);
            dbContext.SaveChanges();
        }
    }
}