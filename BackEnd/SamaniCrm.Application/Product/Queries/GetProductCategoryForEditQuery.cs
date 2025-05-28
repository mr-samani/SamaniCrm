using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Product.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Product.Queries
{
    public record GetProductCategoryForEditQuery(Guid Id) : IRequest<ProductCategoryDto>;

    public class GetProductCategoryForEditQueryHandler : IRequestHandler<GetProductCategoryForEditQuery, ProductCategoryDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetProductCategoryForEditQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductCategoryDto> Handle(GetProductCategoryForEditQuery request, CancellationToken cancellationToken)
        {
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

            return new ProductCategoryDto
            {
                Id = cat.Id,
                OrderIndex = cat.OrderIndex,
                IsActive = cat.IsActive,
                ParentId = cat.ParentId,
                Image = cat.Image,
                Slug = cat.Slug,
                Translations = translations
            };
        }
    }
}
