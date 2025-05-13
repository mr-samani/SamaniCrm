using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using System.Linq.Dynamic.Core;


namespace SamaniCrm.Application.Pages.Queries
{
    public class GetFilteredPagesQueryHandler : IRequestHandler<GetFilteredPagesQuery, PaginatedResult<PageDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFilteredPagesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<PageDto>> Handle(GetFilteredPagesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Pages
                .Include(p => p.Translations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Title))
                query = query.Where(p => p.Translations.Any(t => t.Title.Contains(request.Title)));

            if (!string.IsNullOrWhiteSpace(request.Abstract))
                query = query.Where(p => p.Translations.Any(t => t.Abstract.Contains(request.Abstract)));

            if (!string.IsNullOrWhiteSpace(request.AuthorName))
                query = query.Where(p => p.CreatedBy != null && p.CreatedBy.Contains(request.AuthorName));

            if (request.FromDate.HasValue)
                query = query.Where(p => p.CreationTime >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.CreationTime <= request.ToDate);

            if (request.Status.HasValue)
                query = query.Where(p => p.Status == request.Status);

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortString = $"{request.SortBy} {request.SortDirection}";
                query = query.OrderBy(sortString);
            }

            var result= await query
                .Select(p => new PageDto
                {
                    Id = p.Id,
                    Slag = p.Slag,
                    Status = p.Status,
                    Author = p.CreatedBy,
                    Created = p.CreationTime,
                    Title = p.Translations.FirstOrDefault().Title,
                    Abstract = p.Translations.FirstOrDefault().Abstract
                }).ToListAsync(cancellationToken);

            return new PaginatedResult<PageDto>
            {
                Items = result,
                TotalCount = await query.CountAsync(cancellationToken),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
