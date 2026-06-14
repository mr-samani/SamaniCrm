using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public record GetProductTypeForEditQuery(Guid Id) : IRequest<ProductTypeDto>;

    public class GetProductTypeForEditQueryHandler : IRequestHandler<GetProductTypeForEditQuery, ProductTypeDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILanguageService _languageService;
        public GetProductTypeForEditQueryHandler(
            IApplicationDbContext dbContext, 
            ILanguageService languageService)
        {
            _dbContext = dbContext;
            _languageService = languageService;
        }
        public async Task<ProductTypeDto> Handle(GetProductTypeForEditQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ProductTypes
                       .AsNoTracking()
                       .Where(x => x.Id == request.Id)
                       .Select(s => new ProductTypeDto()
                       {
                           Id = s.Id,

                           Attributes = s.Attributes
                               .OrderBy(x => x.SortOrder)
                               .Select(ss => new ProductAttributeDto()
                               {
                                   Id = ss.Id,
                                   DataType = ss.DataType,
                                   IsRequired = ss.IsRequired,
                                   IsVariant = ss.IsVariant,
                                   ProductTypeId = ss.ProductTypeId,
                                   SortOrder = ss.SortOrder,
                               })
                               .ToList()
                       })
                       .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
                throw new NotFoundException("ProductType not found.");

            var allLangs = await _languageService.GetAllActiveLanguages();

            var translationDict = await _dbContext.ProductTypeTranslations
                .AsNoTracking()
                .Where(x => x.ProductTypeId == request.Id)
                .ToDictionaryAsync(
                    x => x.Culture,
                    x => new
                    {
                        x.Name,
                        x.Description
                    },
                    cancellationToken);

            result.Translations = allLangs
                    .Select(lang =>
                    {
                        translationDict.TryGetValue(lang.Culture, out var trans);

                        return new ProductTypeTranslationDto
                        {
                            Culture = lang.Culture,
                            ProductTypeId = result.Id,
                            Name = trans?.Name ?? string.Empty,
                            Description = trans?.Description ?? string.Empty
                        };
                    })
                    .ToList();

            return result;
        }
    }
}
