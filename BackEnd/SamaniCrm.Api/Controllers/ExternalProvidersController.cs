using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.SecuritySetting.Commands;
using SamaniCrm.Application.SecuritySetting.Queries;
using SamaniCrm.Core.Permissions;
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

        [HttpPost]
        [HttpGet("CreateExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersCreate)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateExternalProviderDto dto)
        {
            var command = new CreateExternalProviderCommand
            {
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                Scheme = dto.Scheme,
                ProviderType = dto.ProviderType,
                ClientId = dto.ClientId,
                ClientSecret = dto.ClientSecret,
                AuthorizationEndpoint = dto.AuthorizationEndpoint,
                TokenEndpoint = dto.TokenEndpoint,
                UserInfoEndpoint = dto.UserInfoEndpoint,
                CallbackPath = dto.CallbackPath,
                LogoutEndpoint = dto.LogoutEndpoint,
                MetadataJson = dto.MetadataJson,
                Scopes = dto.Scopes,
                ResponseType = dto.ResponseType,
                ResponseMode = dto.ResponseMode,
                UsePkce = dto.UsePkce
            };

            var result = await _mediator.Send(command);
            return ApiOk<Guid>(result);
        }

        [HttpPut("{id}")]
        [HttpGet("UpdateExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersUpdate)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExternalProviderDto dto)
        {
            var command = new UpdateExternalProviderCommand
            {
                Id = id,
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                Scheme = dto.Scheme,
                ProviderType = dto.ProviderType,
                ClientId = dto.ClientId,
                ClientSecret = dto.ClientSecret,
                AuthorizationEndpoint = dto.AuthorizationEndpoint,
                TokenEndpoint = dto.TokenEndpoint,
                UserInfoEndpoint = dto.UserInfoEndpoint,
                CallbackPath = dto.CallbackPath,
                LogoutEndpoint = dto.LogoutEndpoint,
                MetadataJson = dto.MetadataJson,
                Scopes = dto.Scopes,
                ResponseType = dto.ResponseType,
                ResponseMode = dto.ResponseMode,
                UsePkce = dto.UsePkce
            };

            var result = await _mediator.Send(command);

            return ApiOk<bool>(result);
        }

        [HttpDelete("{id}")]
        [HttpGet("DeleteExternalProvider")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersDelete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteExternalProviderCommand { Id = id };
            var result = await _mediator.Send(command);
            return ApiOk<bool>(result);
        }

        [HttpGet("{id}")]
        [HttpGet("GetExternalProviderById")]
        [Permission(AppPermissions.SecuritySetting_ExternalProvidersDelete)]
        [ProducesResponseType(typeof(ApiResponse<ExternalProviderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetExternalProviderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return ApiOk<ExternalProviderDto>(result);
        }
    }
}
