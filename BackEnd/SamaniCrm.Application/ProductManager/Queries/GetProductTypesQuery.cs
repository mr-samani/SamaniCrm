using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetProductTypesQuery : IRequest<List<ProductTypeDto>>
    {
        public Guid? TenantId { get; set; }
    }

    public class GetProductTypesQueryHandler : IRequestHandler<GetProductTypesQuery, List<ProductTypeDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;
        public GetProductTypesQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }
        public async Task<List<ProductTypeDto>> Handle(GetProductTypesQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;

            var query = _dbContext.ProductTypes
                .Include(x => x.Attributes)
                .Where(x => x.Culture == currentLanguage)
                .AsQueryable();
            if (request.TenantId.HasValue)
                query = query.Where(x => x.TenantId == request.TenantId.Value);
            var list = await query.ToListAsync(cancellationToken);
            return list.Select(entity => new ProductTypeDto
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                Name = entity.Name,
                Description = entity.Description,

            }).ToList();
        }
    }
}
