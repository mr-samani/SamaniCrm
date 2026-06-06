using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;

namespace SamaniCrm.Infrastructure.Persistence;

public static class SeedPages
{
    public static async Task TrySeedAsync(TenantDbContext dbContext, Guid? tenantId)
    {
        var productPages = await dbContext.Pages
            .IgnoreQueryFilters()
            .Where(p => p.TenantId == tenantId && p.Type == PageTypeEnum.Product)
            .ToListAsync();


        Console.WriteLine("product page count:" + productPages.Count);

        if (!productPages.Any())
        {

            var list = new List<Page>();
            list.Add(new Page()
            {
                TenantId = tenantId,
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                    new PageTranslation(){
                        TenantId = tenantId,
                        Culture = "fa-IR",
                        Title = "دسته بندی محصولات",
                        Description = "",
                    },
                    new PageTranslation(){
                        TenantId = tenantId,
                        Culture = "en-US",
                        Title = "Category of products",
                        Description = "",
                    },
                    ]
            });

            list.Add(new Page()
            {
                TenantId = tenantId,
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                new PageTranslation(){
                        TenantId = tenantId,
                        Culture = "fa-IR",
                        Title = "محصولات",
                        Description = "",
                    },
                    new PageTranslation(){
                        TenantId = tenantId,
                        Culture = "en-US",
                        Title = "Products",
                        Description = "",
                    },
                    ]
            });


            list.Add(new Page()
            {
                TenantId = tenantId,
                Type = PageTypeEnum.Product,
                IsActive = true,
                IsSystem = true,
                Status = PageStatusEnum.Published,
                PublishedAt = DateTime.Now,
                Translations = [
                new PageTranslation(){
                        TenantId = tenantId,
                        Culture = "fa-IR",
                        Title = "سبد خرید",
                        Description = "",
                    },
                    new PageTranslation(){
                        TenantId = tenantId,
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