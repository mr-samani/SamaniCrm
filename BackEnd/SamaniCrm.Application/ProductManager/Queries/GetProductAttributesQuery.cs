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
    public class GetProductAttributesQuery : IRequest<List<ProductAttributeDto>>
    {
        public Guid? ProductTypeId { get; set; }
    }

    public class GetProductAttributesQueryHandler : IRequestHandler<GetProductAttributesQuery, List<ProductAttributeDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetProductAttributesQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ProductAttributeDto>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.ProductAttributes
                .Include(x => x.Translations)
                .AsQueryable();
            if (request.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == request.ProductTypeId.Value);
            var list = await query.ToListAsync(cancellationToken);
            return list.Select(entity => new ProductAttributeDto
            {
                Id = entity.Id,
                ProductTypeId = entity.ProductTypeId,
                DataType = entity.DataType,
                IsRequired = entity.IsRequired,
                IsVariant = entity.IsVariant,
                SortOrder = entity.SortOrder,
                Translations = entity.Translations?.Select(t => new ProductAttributeTranslationDto
                {
                    ProductAttributeId = t.ProductAttributeId,
                    Culture = t.Culture,
                    Name = t.Name
                }).ToList()
            }).ToList();
        }
    }
}
