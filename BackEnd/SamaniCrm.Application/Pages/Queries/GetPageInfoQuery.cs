﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Pages.Queries
{
    public record GetPageInfoQuery(Guid PageId, string Culture) : IRequest<PageDto>;

    public class GetPageInfoQueryHandler : IRequestHandler<GetPageInfoQuery, PageDto>
    {
        private readonly IPageService _pageService;

        public GetPageInfoQueryHandler(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<PageDto> Handle(GetPageInfoQuery request, CancellationToken cancellationToken)
        {
            return await _pageService.GetPageInfo(request, cancellationToken);
        }
    }

}
