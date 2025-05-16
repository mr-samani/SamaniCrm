using System.Linq.Dynamic.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class PagesController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public PagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("GetAllPages")]
        [Permission(AppPermissions.Pages_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PageDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPages(GetFilteredPagesQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<PageDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetForEditMetaData")]
        [Permission(AppPermissions.Pages_Update)]
        [ProducesResponseType(typeof(ApiResponse<PageForEditDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForEditMetaData(Guid pageId, CancellationToken cancellationToken)
        {
            PageForEditDto result = await _mediator.Send(new GetPageForEditMetaDataQuery(pageId), cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetPageInfo")]
        [Permission(AppPermissions.Pages_Update)]
        [ProducesResponseType(typeof(ApiResponse<PageDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPageInfo(Guid pageId, string culture, CancellationToken cancellationToken)
        {
            PageDto result = await _mediator.Send(new GetPageInfoQuery(pageId, culture), cancellationToken);
            return ApiOk(result);
        }



        [HttpPost("CreateOrEditPageMetaData")]
        [Permission(AppPermissions.Pages_Create)]
        [Permission(AppPermissions.Pages_Update)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditPageMetaData(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken)
        {
            Guid result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("UpdatePageContent")]
        [Permission(AppPermissions.Pages_Create)]
        [Permission(AppPermissions.Pages_Update)]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePageContent(UpdatePageContentCommand request, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("DeletePage")]
        [Permission(AppPermissions.Pages_Delete)]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePage(DeletePageCommand request, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


    }
}
