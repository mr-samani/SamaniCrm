using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;



namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetProductAttributesQuery : PaginationRequest, IRequest<PaginatedResult<ProductAttributeDto>>
    {
        public string? Filter { get; set; }

        public Guid? ProductTypeId { get; set; }
    }

    public class GetProductAttributesQueryHandler : IRequestHandler<GetProductAttributesQuery, PaginatedResult<ProductAttributeDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;

        public GetProductAttributesQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }
        public async Task<PaginatedResult<ProductAttributeDto>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;

            var query = _dbContext.ProductAttributes
                .Include(x => x.Translations)
                .AsQueryable();
            if (request.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == request.ProductTypeId.Value);



            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(c =>
                            c.Translations.Any(t => t.Culture == currentLanguage &&
                            (t.Name.Contains(request.Filter))
                            )
                    );
            }

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortString = $"{request.SortBy} {request.SortDirection}";
                query = query.OrderBy(sortString);
            }

            int total = await query.CountAsync(cancellationToken);




            var items = await query
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .Select(s => new ProductAttributeDto()
                {
                    Id = s.Id,
                    ProductTypeId = s.ProductTypeId,
                    DataType = s.DataType,
                    IsRequired = s.IsRequired,
                    IsVariant = s.IsVariant,
                    SortOrder = s.SortOrder,
                    Name = s.Translations.Select(s => s.Name).FirstOrDefault() ?? "",
                })
                .ToListAsync(cancellationToken);
            return new PaginatedResult<ProductAttributeDto>()
            {

                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };

        }
    }
}
