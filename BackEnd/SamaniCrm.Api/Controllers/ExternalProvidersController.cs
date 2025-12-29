using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.SecuritySetting.Commands;
using SamaniCrm.Application.SecuritySetting.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class ExternalProvidersController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ExternalProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllExternalProviders")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersList)]
        [ProducesResponseType(typeof(ApiResponse<List<ExternalProviderDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExternalProviders()
        {
            List<ExternalProviderDto> result = await _mediator.Send(new GetExternalProvidersQuery(false));
            return ApiOk(result);
        }

        [HttpPost("ChangeIsActiveExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersUpdate)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeIsActiveExternalProvider(Guid id, bool isActive)
        {
            bool result = await _mediator.Send(new ChangeIsActiveExternalProviderCommand(id, isActive));
            return ApiOk(result);
        }


        [HttpPost("CreateExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersCreate)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateExternalProviderCommand command)
        {
            var result = await _mediator.Send(command);
            return ApiOk<Guid>(result);
        }

        [HttpPost("UpdateExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersUpdate)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateExternalProviderCommand command)
        {
            var result = await _mediator.Send(command);

            return ApiOk<bool>(result);
        }

        [HttpDelete("DeleteExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersDelete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteExternalProviderCommand { Id = id };
            var result = await _mediator.Send(command);
            return ApiOk<bool>(result);
        }

        [HttpGet("GetExternalProviderById")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersDelete)]
        [ProducesResponseType(typeof(ApiResponse<CreateOrUpdateExternalProviderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetExternalProviderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return ApiOk<CreateOrUpdateExternalProviderDto>(result);
        }
    }
}
