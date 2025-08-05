using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs.PageBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Pages.Queries;

public record GetCustomBlockQuery : IRequest<List<CustomBlockDto>>;



public class GetCustomBlockQueryHandler : IRequestHandler<GetCustomBlockQuery, List<CustomBlockDto>>
{
    private readonly IPageService _pageService;

    public GetCustomBlockQueryHandler(IPageService pageService)
    {
        _pageService = pageService;
    }

    public Task<List<CustomBlockDto>> Handle(GetCustomBlockQuery request, CancellationToken cancellationToken)
    {
        return _pageService.GetCustomBlocks(request, cancellationToken);
    }
}