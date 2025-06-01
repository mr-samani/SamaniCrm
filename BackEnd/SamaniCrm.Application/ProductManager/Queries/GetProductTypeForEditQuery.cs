using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
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
        public GetProductTypeForEditQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductTypeDto> Handle(GetProductTypeForEditQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.ProductTypes
                .Include(x => x.Attributes)
                .Where(x => x.Id == request.Id)
                .Select(s => new ProductTypeDto()
                {
                    Id = s.Id,
                    Attributes = (List<ProductAttributeDto>)s.Attributes.Select(ss => new ProductAttributeDto()
                    {
                        Id = ss.Id,
                        DataType = ss.DataType,
                        IsRequired = ss.IsRequired,
                        IsVariant = ss.IsVariant,
                        ProductTypeId = ss.ProductTypeId,
                        SortOrder = ss.SortOrder,
                    }),
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                throw new NotFoundException("ProductType not found.");

            List<ProductTypeTranslationDto> translations = await _dbContext.Languages
                .GroupJoin(_dbContext.ProductTypeTranslations.Where(w => w.ProductTypeId == request.Id),
                  lang => lang.Culture,
                  translation => translation.Culture,
                  (lang, trans) => new { lang, trans }
                )
                .SelectMany(
                x => x.trans.DefaultIfEmpty(),
                (x, trans) => new ProductTypeTranslationDto()
                {
                    Culture = x.lang.Culture,
                    ProductTypeId = request.Id,
                    Name = trans != null ? trans.Name : "",
                    Description = trans != null ? trans.Description : "",
                }
                ).ToListAsync(cancellationToken);
            result.Translations = translations;
            return result;
        }
    }
}
