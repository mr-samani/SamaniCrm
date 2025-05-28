using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public record GetProductForEditQuery(Guid Id) : IRequest<ProductDto>;

    public class GetProductForEditQueryHandler : IRequestHandler<GetProductForEditQuery, ProductDto>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetProductForEditQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductDto> Handle(GetProductForEditQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Products
                .Include(x => x.Translations)
                .Include(x => x.Images)
                .Include(x => x.Files)
                .Include(x => x.Prices)
                .Include(x => x.AttributeValues)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Product not found.");
            var dto = new ProductDto
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
            };
            return dto;
        }
    }
}
