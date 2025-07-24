using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Queries;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities.ProductEntities;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;


namespace SamaniCrm.Infrastructure.Services.Product;

public class ProductCategoryService : IProductCategoryService
{
    private readonly ApplicationDbContext _context;

    private readonly ICurrentUserService _currentUserService;
    private readonly ILocalizer L;

    public ProductCategoryService(ApplicationDbContext context, ICurrentUserService currentUserService, ILocalizer l)
    {
        _context = context;
        _currentUserService = currentUserService;
        L = l;
    }



    public async Task<List<ProductCategoryDto>> GetCategoryTree(CancellationToken cancellationToken)
    {
        var currentLanguage = L.CurrentLanguage;
        var allCategories = await _context.ProductCategories.Include(m => m.Translations)
                        .Include(m => m.Children)
                            .ThenInclude(c => c.Translations)
                        .OrderBy(m => m.OrderIndex)
                        .ToListAsync();
        var roots = allCategories.Where(m => m.ParentId == null).ToList();
        var result = roots.Select(m => MapToDtoRecursive(m, currentLanguage)).ToList();
        return result ?? [];
    }



    private static ProductCategoryDto MapToDtoRecursive(ProductCategory item, string language)
    {
        return new ProductCategoryDto
        {
            Id = item.Id,
            Image = item.Image,
            OrderIndex = item.OrderIndex,
            ParentId = item.ParentId,
            Slug = item.Slug,
            IsActive = item.IsActive,
            Title = item.Translations?.FirstOrDefault(t => t.Culture == language)?.Title ?? "",
            Description = item.Translations?.FirstOrDefault(t => t.Culture == language)?.Description ?? "",
            Children = item.Children?
                .OrderBy(c => c.OrderIndex)
                .Select(c => MapToDtoRecursive(c, language))
                .ToList() ?? []
        };
    }

    public async Task<PagedProductCategoriesDto> GetPagedCategories(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
    {

        var currentLanguage = L.CurrentLanguage;

        IQueryable<ProductCategory> query = _context.ProductCategories
       .Where(c => c.ParentId == (request.ParentId ?? null))
       .Include(c => c.Translations);

        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(c =>
           c.Translations.Any(t => t.Culture == currentLanguage && t.Title.Contains(request.Filter)));
        }

        // Sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortString = $"{request.SortBy} {request.SortDirection}";
            query = query.OrderBy(sortString);
        }

        int total = await query.CountAsync(cancellationToken);

        var items = await query
       .Skip(request.PageSize * (request.PageNumber - 1))
       .Take(request.PageSize)
       .Select(c => new PagedProductCategoryDto
       {
           Id = c.Id,
           Title = c.Translations
                       .Where(t => t.Culture == currentLanguage)
                       .Select(t => t.Title)
                       .FirstOrDefault() ?? "",
           Description = c.Translations
                       .Where(t => t.Culture == currentLanguage)
                       .Select(t => t.Description)
                       .FirstOrDefault() ?? "",
           Image = c.Image,
           IsActive = c.IsActive,
           OrderIndex = c.OrderIndex,
           ParentId = c.ParentId,
           Slug = c.Slug,
           CreationTime = c.CreationTime.ToUniversalTime(),

           // فقط چک کن که فرزند داره یا نه، بدون کوئری اضافه
           ChildCount = _context.ProductCategories.Count(x => x.ParentId == c.Id),
           hasChild = _context.ProductCategories.Any(x => x.ParentId == c.Id)
       })
       .ToListAsync(cancellationToken);


        //Dictionary<Guid, string> breadcrumbs = new();
        //if (request.ParentId != null)
        //{
        //    var id = request.ParentId;
        //    while (id != null)
        //    {
        //        var found = await _context.ProductCategoryTranslations
        //            .Include(t => t.ProductCategory)
        //            .Select(s => new
        //            {
        //                Id = s.CategoryId,
        //                Title = s.Title,
        //                Culture = s.Culture,
        //                ParentId = s.ProductCategory.ParentId
        //            })
        //            .Where(x => x.Id == id && x.Culture == currentLanguage)
        //            .FirstOrDefaultAsync(cancellationToken);
        //        if (found != null)
        //        {
        //            breadcrumbs.Add(found.Id, found.Title);
        //            id = found.ParentId;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}


        var sql = @"
                  WITH CategoryCTE AS (
                        SELECT pc.Id, pc.ParentId
                        FROM product.ProductCategories pc
                        WHERE pc.Id = @parentId

                        UNION ALL

                        SELECT parent.Id, parent.ParentId
                        FROM product.ProductCategories parent
                        INNER JOIN CategoryCTE cte ON parent.Id = cte.ParentId
                    )
                    SELECT cte.Id, ISNULL(t.Title, '') AS Title
                    FROM CategoryCTE cte
                    LEFT JOIN product.ProductCategoryTranslations t 
                        ON t.CategoryId = cte.Id AND t.Culture = @culture;
            ";

        var breadcrumbs = await _context.Database
            .SqlQueryRaw<BreadcrumbResult>(sql,
                new SqlParameter("@parentId", request.ParentId ?? (object)DBNull.Value),
                new SqlParameter("@culture", currentLanguage))
            .ToListAsync(cancellationToken);
        //.ToDictionaryAsync(d => d.Id, d => d.Title, cancellationToken);


        return new PagedProductCategoriesDto()
        {
            Items = items,
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Breadcrumbs = breadcrumbs
        };
    }

    public async Task<List<ProductCategoryDto>> GetPublicCategories(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {

        var currentLanguage = L.CurrentLanguage;

        IQueryable<ProductCategory> query = _context.ProductCategories
            .Where(w => w.IsActive == true)
            .Where(c => c.ParentId == (request.ParentId ?? null))
            .Include(c => c.Translations);

        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(c =>
           c.Translations.Any(t => t.Culture == currentLanguage && t.Title.Contains(request.Filter)));
        }



        List<ProductCategoryDto> items = await query
       .Skip(request.Skip)
       .Take(request.Take)
       .Select(c => new ProductCategoryDto
       {
           Id = c.Id,
           Title = c.Translations
                       .Where(t => t.Culture == currentLanguage)
                       .Select(t => t.Title)
                       .FirstOrDefault() ?? "",
           Description = c.Translations
                       .Where(t => t.Culture == currentLanguage)
                       .Select(t => t.Description)
                       .FirstOrDefault() ?? "",
           Image = c.Image,
           OrderIndex = c.OrderIndex,
           ParentId = c.ParentId,
           Slug = c.Slug,
       })
       .ToListAsync(cancellationToken);

        return items;

    }
}

