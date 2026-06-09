using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Pages.Queries
{
    public record GetPageInfoQuery(string Culture, Guid? PageId, string? slug) : IRequest<PageDto>;

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
