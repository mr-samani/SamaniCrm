using System.Linq.Dynamic.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;
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
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PageDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPages(GetFilteredPagesQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<PageDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("CreatePage")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePage(CreatePageCommand request, CancellationToken cancellationToken)
        {
            Guid result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("UpdatePage")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePage(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("DeletePage")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePage(DeletePageCommand request, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


    }
}
