using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetProductTypesQuery : PaginationRequest, IRequest<PaginatedResult<ProductTypeDto>>
    {
        public string? Filter { get; set; }
    }

    public class GetProductTypesQueryHandler : IRequestHandler<GetProductTypesQuery, PaginatedResult<ProductTypeDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;
        public GetProductTypesQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }
        public async Task<PaginatedResult<ProductTypeDto>> Handle(GetProductTypesQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;

            var query = _dbContext.ProductTypes
                .Include(x => x.Translations)
                .AsQueryable();


            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(c =>
                            c.Translations.Any(t => t.Culture == currentLanguage &&
                            (t.Name.Contains(request.Filter) || t.Description.Contains(request.Filter))
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
                .Select(s => new ProductTypeDto()
                {
                    Id = s.Id,
                    Name = s.Translations.Where(w => w.Culture == currentLanguage).Select(s => s.Name).FirstOrDefault() ?? "",
                    Description = s.Translations.Where(w => w.Culture == currentLanguage).Select(s => s.Description).FirstOrDefault() ?? "",
                    CreationTime = s.CreationTime,
                })
                .ToListAsync(cancellationToken);
            return new PaginatedResult<ProductTypeDto>()
            {

                Items = items,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }
}
