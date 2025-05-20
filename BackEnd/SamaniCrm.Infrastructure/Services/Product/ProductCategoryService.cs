using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities.ProductEntities;

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
                var allCategories =await _context.ProductCategories.Include(m => m.Translations)
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

    }

