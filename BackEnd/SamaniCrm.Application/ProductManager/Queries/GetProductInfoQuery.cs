using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries
{
    public record GetProductInfoQuery(string Slug, string? Culture) : IRequest<ProductInfoDto>;


    public class GetProductInfoQueryHandler : IRequestHandler<GetProductInfoQuery, ProductInfoDto>
    {
        private readonly ILocalizer L;
        private readonly IApplicationDbContext _dbContext;

        public GetProductInfoQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }


        public async Task<ProductInfoDto> Handle(GetProductInfoQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = request.Culture ?? L.CurrentLanguage;

            var entity = await _dbContext.Products
                .AsNoTracking()
                .Include(x => x.Category)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.ProductType)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.Translations)
                .Include(x => x.Images)
                .Include(x => x.Files)
                .Include(x => x.Prices)
                .Include(x => x.AttributeValues)
                    .ThenInclude(x => x.Attribute)
                        .ThenInclude(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (entity == null)
                throw new NotFoundException("Product not found.");

            var translation = entity.Translations.FirstOrDefault(x => x.Culture == currentLanguage);
            var categoryTranslation = entity.Category?.Translations.FirstOrDefault(x => x.Culture == currentLanguage);
            var typeTranslation = entity.ProductType?.Translations.FirstOrDefault(x => x.Culture == currentLanguage);

            var dto = new ProductInfoDto
            {
                Id = entity.Id,
                CategoryTitle = categoryTranslation?.Title ?? "",
                ProductTypeTitle = typeTranslation?.Name ?? "",
                SKU = entity.SKU?.ToString() ?? "",
                Slug = entity.Slug,
                IsActive = entity.IsActive,
                Tags = entity.Tags ?? "",
                Title = translation?.Title ?? "",
                Description = translation?.Description ?? "",
                Content = translation?.Content ?? "",

                Images = entity.Images?.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    FileId = img.FileId,
                    IsMain = img.IsMain,
                    SortOrder = img.SortOrder
                }).ToList() ?? new(),

                Files = entity.Files?.Select(file => new ProductFileDto
                {
                    Id = file.Id,
                    FileId = file.FileId,
                    Description = file.Description
                }).ToList() ?? new(),

                Price = entity.Prices?.Where(w => w.EndDate >= DateTime.Now).Select(p => p.Price).FirstOrDefault() ?? 0,

                AttributeValues = entity.AttributeValues?.Select(attr => new ProductAttributeInfoDto
                {
                    AttributeId = attr.AttributeId,
                    Title = attr.Attribute?.Translations?.FirstOrDefault(x => x.Culture == currentLanguage)?.Name ?? "",
                    Value = attr.Value?.Value ?? "",
                    DataType = attr.Attribute?.DataType ?? ProductAttributeDataTypeEnum.String,
                }).ToList() ?? new()
            };

            return dto;
        }
    }

}
