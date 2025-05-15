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
using Microsoft.EntityFrameworkCore.Internal;


namespace SamaniCrm.Application.Pages.Queries
{
    public class GetFilteredPagesQueryHandler : IRequestHandler<GetFilteredPagesQuery, PaginatedResult<PageDto>>
    {
        private readonly IPageService _pageService;

        public GetFilteredPagesQueryHandler(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<PaginatedResult<PageDto>> Handle(GetFilteredPagesQuery request, CancellationToken cancellationToken)
        {
            return await _pageService.GetPagedList(request, cancellationToken);
        }
    }
}
