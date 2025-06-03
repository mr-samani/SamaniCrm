using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries
{
    public record GetProductInfoQuery(string Slug, string? Culture) : IRequest<ProductDto>;


    public class GetProductInfoQueryHandler : IRequestHandler<GetProductInfoQuery, ProductDto>
    {
        private readonly ILocalizer L;
        private readonly IApplicationDbContext _dbContext;

        public GetProductInfoQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }


        public async Task<ProductDto> Handle(GetProductInfoQuery request, CancellationToken cancellationToken)
        {
            var currentLangugage = request.Culture ?? L.CurrentLanguage;
            var entity = await _dbContext.Products
                .Include(x => x.Category)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.ProductType)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.Translations)
                .Include(x => x.Images)
                .Include(x => x.Files)
                .Include(x => x.Prices)
                .Include(x => x.AttributeValues)
                // .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Product not found.");
            var dto = new ProductDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                CategoryTitle = entity.Category.Translations.Where(w => w.Culture == currentLangugage).Select(x => x.Title).FirstOrDefault(),
                ProductTypeId = entity.ProductTypeId,
                ProductTypeTitle = entity.ProductType.Translations.Where(w => w.Culture == currentLangugage).Select(x => x.Name).FirstOrDefault(),
                SKU = entity.SKU.ToString(),
                Slug = entity.Slug,
                IsActive = entity.IsActive,
                Tags = entity.Tags,
                Title = entity.Translations.Where(w => w.Culture == currentLangugage).Select(x => x.Title).FirstOrDefault() ?? "",
                Description = entity.Translations.Where(w => w.Culture == currentLangugage).Select(x => x.Description).FirstOrDefault() ?? "",
                Content = entity.Translations.Where(w => w.Culture == currentLangugage).Select(x => x.Content).FirstOrDefault() ?? "",


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
