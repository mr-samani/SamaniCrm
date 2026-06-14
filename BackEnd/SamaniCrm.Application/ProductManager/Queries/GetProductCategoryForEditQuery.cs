using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
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

        private readonly ILanguageService _languageService;

        public GetProductCategoryForEditQueryHandler(
            IApplicationDbContext dbContext, ILocalizer l,
            ILanguageService languageService)
        {
            _dbContext = dbContext;
            L = l;
            _languageService = languageService;
        }

        public async Task<ProductCategoryDto> Handle(GetProductCategoryForEditQuery request, CancellationToken cancellationToken)
        {
            var currentLangugage = L.CurrentLanguage;

            var cat = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (cat == null)
                throw new NotFoundException("Category not found.");

            var allLangs = await _languageService.GetAllActiveLanguages();


            var translationDict = await _dbContext.ProductCategoryTranslations
                .Where(w => w.CategoryId == request.Id)
                 .Select(x => new
                 {
                     x.Culture,
                     x.Title
                 })
                .ToDictionaryAsync(x => x.Culture, x => x.Title, cancellationToken);

            string? parentTitle = "";
            if (cat.ParentId.HasValue)
            {
                parentTitle = await _dbContext.ProductCategoryTranslations.Where(
                        x => x.CategoryId == cat.ParentId &&
                        x.Culture == currentLangugage
                    ).Select(s => s.Title
                    ).FirstOrDefaultAsync(cancellationToken);
            }


            var translations = allLangs.Select(lang => new ProductCategoryTranslationDto
            {
                Culture = lang.Culture,
                ProductCategoryId = cat.Id,
                Title = translationDict.TryGetValue(lang.Culture, out var title) ? title : string.Empty
            }).ToList();


            return new ProductCategoryDto
            {
                Id = cat.Id,
                OrderIndex = cat.OrderIndex,
                IsActive = cat.IsActive,
                ParentId = cat.ParentId,
                ParentTitle = parentTitle,
                Image = cat.Image,
                Slug = cat.Slug,
                Translations = translations
            };
        }
    }
}
