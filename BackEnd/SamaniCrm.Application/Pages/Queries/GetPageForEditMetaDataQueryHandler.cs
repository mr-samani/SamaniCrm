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
        private readonly IPageService _pageService;

        public GetPageForEditMetaDataQueryHandler(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<PageForEditDto> Handle(GetPageForEditMetaDataQuery request, CancellationToken cancellationToken)
        {
            return await _pageService.GetPageForEdit(request, cancellationToken);
        }
    }
}
