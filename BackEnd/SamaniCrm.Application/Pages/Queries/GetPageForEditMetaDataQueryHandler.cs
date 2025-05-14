using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Pages.Queries
{
    public record GetPageForEditMetaDataQuery(Guid pageId) : IRequest<PageForEditDto>;
    public class GetPageForEditMetaDataQueryHandler : IRequestHandler<GetPageForEditMetaDataQuery, PageForEditDto>
    {
        private readonly IApplicationDbContext _context;

        public GetPageForEditMetaDataQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PageForEditDto> Handle(GetPageForEditMetaDataQuery request, CancellationToken cancellationToken)
        {
            var page = await _context.Pages
                .Select(s => new PageForEditDto()
                {
                    Id = s.Id,
                    CoverImage = s.CoverImage,
                    IsActive = s.IsActive,
                    IsSystem = s.IsSystem,
                    Status = s.Status,
                    Type = s.Type,
                    Translations = s.Translations.Select(t => new PageMetaDataDto()
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Culture = t.Culture,
                        Abstract = t.Abstract,
                        Description = t.Description,
                        MetaDescription = t.MetaDescription,
                        MetaKeywords = t.MetaKeywords,
                    }).ToList()
                })
                .Where(x => x.Id == request.pageId)
                .Include(x => x.Translations)
                .FirstOrDefaultAsync();
            if (page == null)
            {
                throw new NotFoundException("Page not found");
            }

            return page;
        }
    }
}
