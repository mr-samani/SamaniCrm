using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.DTOs.PageBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Queries;

public class GetPluginQuery : PaginationRequest, IRequest<PaginatedResult<PluginDto>>
{
    public string? Filter { get; set; }
}



public class GetPluginQueryHandler : IRequestHandler<GetPluginQuery, PaginatedResult<PluginDto>>
{
    private readonly IPageService _pageService;

    public GetPluginQueryHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public Task<PaginatedResult<PluginDto>> Handle(GetPluginQuery request, CancellationToken cancellationToken)
    {
        return _pageService.GetPlugins(request, cancellationToken);
    }
}