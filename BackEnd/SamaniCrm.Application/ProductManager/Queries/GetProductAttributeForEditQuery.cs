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
    public record GetProductAttributeForEditQuery(Guid Id) : IRequest<ProductAttributeDto>;

    public class GetProductAttributeForEditQueryHandler : IRequestHandler<GetProductAttributeForEditQuery, ProductAttributeDto>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetProductAttributeForEditQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductAttributeDto> Handle(GetProductAttributeForEditQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.ProductAttributes
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("ProductAttribute not found.");
            var dto = new ProductAttributeDto
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
            };
            return dto;
        }
    }
}
