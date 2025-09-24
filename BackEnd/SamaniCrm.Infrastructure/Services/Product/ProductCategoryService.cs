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
        var currentCulture = L.CurrentLanguage;

        var items = await _context.ProductCategories
                 .Where(w => w.IsActive)
                 .Where(c => c.ParentId == (request.ParentId ?? null))
                 .Where(c => string.IsNullOrEmpty(request.Filter) ||
                             c.Translations.Any(t => t.Culture == currentCulture && t.Title.Contains(request.Filter)))
                 .OrderBy(c => c.OrderIndex)
                 .Skip(request.Skip)
                 .Take(request.Take)
                 .Select(c => new
                 {
                     c.Id,
                     c.Image,
                     c.OrderIndex,
                     c.ParentId,
                     c.Slug,
                     Translations = c.Translations
                         .OrderByDescending(t => t.Culture == currentCulture)
                         .Select(t => new { t.Title, t.Description, t.Culture })
                         .ToList()
                 })
                 .ToListAsync(cancellationToken);


        var result = items.Select(c =>
        {
            var best = c.Translations.FirstOrDefault();
            return new ProductCategoryDto
            {
                Id = c.Id,
                Title = best?.Title ?? "",
                Description = best?.Description ?? "",
                Image = c.Image,
                OrderIndex = c.OrderIndex,
                ParentId = c.ParentId,
                Slug = c.Slug,
            };
        }).ToList();

        return result;
    }

    public async Task<List<ProductCategoryDto>> GetTreeProductCategories(bool onlyIsActive, CancellationToken cancellationToken)
    {
        var currentCulture = L.CurrentLanguage;

        var query = _context.ProductCategories
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.ParentId,
                c.Image,
                c.OrderIndex,
                c.Slug,
                c.IsActive,
                PreferredTranslation = c.Translations
                    .Where(t => t.Culture == currentCulture && t.Title != null)
                    .Select(t => new { t.Title, t.Description })
                    .FirstOrDefault(),

                FallbackTranslation = c.Translations
                    .Where(t => t.Title != null)
                    .Select(t => new { t.Title, t.Description })
                    .FirstOrDefault()
            });

        if (onlyIsActive)
        {
            query = query.Where(c => c.IsActive);
        }

        var items = await query
            .OrderBy(c => c.OrderIndex)
            .ToListAsync(cancellationToken);

        // تبدیل به DTO
        var dtoList = items.Select(c => new ProductCategoryDto
        {
            Id = c.Id,
            ParentId = c.ParentId,
            Image = c.Image,
            OrderIndex = c.OrderIndex,
            Slug = c.Slug,
            Title = c.PreferredTranslation?.Title ?? c.FallbackTranslation?.Title ?? "",
            Description = c.PreferredTranslation?.Description ?? c.FallbackTranslation?.Description ?? ""
        }).ToList();

        // ساخت درخت
        var lookup = dtoList.ToLookup(c => c.ParentId);
        foreach (var dto in dtoList)
        {
            dto.Children = lookup[dto.Id].OrderBy(x => x.OrderIndex).ToList();
        }

        var tree = lookup[null].OrderBy(x => x.OrderIndex).ToList();
        return tree;
    }








}

