using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetProductsQuery : PaginationRequest, IRequest<PaginatedResult<ProductDto>>
    {
        public string? Filter { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ProductTypeId { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<ProductDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;
        public GetProductsQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }
        public async Task<PaginatedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;

            var query = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductType)
                .Include(x => x.Images)
                .AsNoTracking()
                .AsQueryable();

            if (request.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == request.CategoryId.Value);

            if (request.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == request.ProductTypeId.Value);

            if (!string.IsNullOrEmpty(request.Filter))
            {
                var filter = request.Filter;
                query = query.Where(c =>
                    c.Translations.Any(t => t.Culture == currentLanguage && (
                        t.Title.Contains(filter) || t.Description.Contains(filter)
                    )) ||
                    c.Category.Translations.Any(cat => cat.Culture == currentLanguage && cat.Title.Contains(filter)) ||
                    c.ProductType.Translations.Any(pt => pt.Culture == currentLanguage && pt.Name.Contains(filter))
                );
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
                .Select(s => new ProductDto
                {
                    Id = s.Id,
                    TenantId = s.TenantId,
                    CategoryId = s.CategoryId,
                    ProductTypeId = s.ProductTypeId,
                    SKU = s.SKU.Value,
                    Slug = s.Slug,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    Title = s.Translations.FirstOrDefault(x => x.Culture == currentLanguage).Title ?? "",
                    Description = s.Translations.FirstOrDefault(x => x.Culture == currentLanguage).Description ?? "",
                    CategoryTitle = s.Category.Translations.Where(w => w.Culture == currentLanguage).Select(s => s.Title).FirstOrDefault() ?? "",
                    ProductTypeTitle = s.ProductType.Translations.Where(x => x.Culture == currentLanguage).Select(s => s.Name).FirstOrDefault() ?? ""
                })
                .ToListAsync(cancellationToken);
            return new PaginatedResult<ProductDto>()
            {

                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }
}
