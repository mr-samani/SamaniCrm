using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public Guid? CategoryId { get; set; }
        public Guid? ProductTypeId { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetProductsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Products
                .Include(x => x.Translations)
                .Include(x => x.Images)
                .Include(x => x.Files)
                .Include(x => x.Prices)
                .Include(x => x.AttributeValues)
                .AsQueryable();
            if (request.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == request.CategoryId.Value);
            if (request.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == request.ProductTypeId.Value);
            var list = await query.ToListAsync(cancellationToken);
            return list.Select(entity => new ProductDto
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                CategoryId = entity.CategoryId,
                ProductTypeId = entity.ProductTypeId,
                SKU = entity.SKU.ToString(),
                Slug = entity.Slug,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                // Translations mapping
                Translations = entity.Translations.Select(t => new ProductTranslationDto
                {
                    ProductId = t.ProductId,
                    Culture = t.Culture,
                    Title = t.Title,
                    Description = t.Description
                }).ToList(),
                Images = entity.Images.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    Url = img.Url,
                    IsMain = img.IsMain,
                    SortOrder = img.SortOrder
                }).ToList(),
                Files = entity.Files.Select(file => new ProductFileDto
                {
                    Id = file.Id,
                    FileUrl = file.FileUrl,
                    FileType = file.FileType
                }).ToList(),
                Prices = entity.Prices.Select(price => new ProductPriceDto
                {
                    Id = price.Id,
                    Currency = price.Currency,
                    Price = price.Price,
                    StartDate = price.StartDate,
                    EndDate = price.EndDate
                }).ToList(),
                AttributeValues = entity.AttributeValues.Select(attr => new ProductAttributeValueDto
                {
                    Id = attr.Id,
                    AttributeId = attr.AttributeId,
                    Value = attr.Value.Value
                }).ToList()
            }).ToList();
        }
    }
}
