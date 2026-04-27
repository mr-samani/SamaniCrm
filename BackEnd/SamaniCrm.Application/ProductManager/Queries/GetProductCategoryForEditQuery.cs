using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public record GetProductCategoryForEditQuery(Guid Id) : IRequest<ProductCategoryDto>;


    public class GetProductCategoryForEditQueryHandler : IRequestHandler<GetProductCategoryForEditQuery, ProductCategoryDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;

        public GetProductCategoryForEditQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }

        public async Task<ProductCategoryDto> Handle(GetProductCategoryForEditQuery request, CancellationToken cancellationToken)
        {
            var currentLangugage = L.CurrentLanguage;

            var cat = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (cat == null)
                throw new NotFoundException("Category not found.");

            List<ProductCategoryTranslationDto> translations = await _dbContext.Languages
                .GroupJoin(_dbContext.ProductCategoryTranslations.Where(w => w.CategoryId == request.Id),
                  lang => lang.Culture,
                  translation => translation.Culture,
                  (lang, trans) => new { lang, trans }
                )
                .SelectMany(
                x => x.trans.DefaultIfEmpty(),
                (x, trans) => new ProductCategoryTranslationDto()
                {
                    Culture = x.lang.Culture,
                    ProductCategoryId = cat.Id,
                    Title = trans != null ? trans.Title : "",
                    Description = trans != null ? trans.Description : "",

                }
                ).ToListAsync(cancellationToken);
            string? parentTitle = "";
            if (cat.ParentId.HasValue)
            {
                parentTitle = await _dbContext.ProductCategoryTranslations.Where(
                        x => x.CategoryId == cat.ParentId &&
                        x.Culture == currentLangugage
                    ).Select(s => s.Title
                    ).FirstOrDefaultAsync(cancellationToken);
            }

            return new ProductCategoryDto
            {
                Id = cat.Id,
                OrderIndex = cat.OrderIndex,
                IsActive = cat.IsActive,
                ParentId = cat.ParentId,
                ParentTitle= parentTitle,
                Image = cat.Image,
                Slug = cat.Slug,
                Translations = translations
            };
        }
    }
}
