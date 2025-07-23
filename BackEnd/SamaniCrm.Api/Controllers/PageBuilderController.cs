using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class PageBuilderController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public PageBuilderController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost("SaveAsBlockDefinition")]
        [Permission(AppPermissions.Pages_CreateCustomBlock)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveAsBlockDefinition(CreateCustomBlockCommand request, CancellationToken cancellationToken)
        {
            Guid result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("DeleteCustomBlock")]
        [Permission(AppPermissions.Pages_DeleteCustomBlock)]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCustomBlock(DeletePageCommand request, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetCustomBlocks")]
        [Permission(AppPermissions.Pages_CustomBlockList)]
        [ProducesResponseType(typeof(ApiResponse<List<CustomBlockDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomBlocks(CancellationToken cancellationToken)
        {
            List<CustomBlockDto> result = await _mediator.Send(new GetCustomBlockQuery(), cancellationToken);
            return ApiOk(result);
        }



    }
}
