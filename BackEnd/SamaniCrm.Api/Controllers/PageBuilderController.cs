using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
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



        [HttpPost("CreatePlugin")]
        [Permission(AppPermissions.Pages_CreatePlugin)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePlugin(CreatePluginCommand request, CancellationToken cancellationToken)
        {
            Guid result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }


        [HttpPost("DeletePlugin")]
        [Permission(AppPermissions.Pages_DeletePlugin)]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePlugin(Guid Id, CancellationToken cancellationToken)
        {
            Unit result = await _mediator.Send(new DeletePluginCommand(Id), cancellationToken);
            return ApiOk(result);
        }

        [HttpPost("GetPlugins")]
        [Permission(AppPermissions.Pages_PluginList)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PluginDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlugins(GetPluginQuery Input,CancellationToken cancellationToken)
        {
            PaginatedResult<PluginDto> result = await _mediator.Send(Input, cancellationToken);
            return ApiOk(result);
        }



    }
}
