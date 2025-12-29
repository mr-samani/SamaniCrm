using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.SecuritySetting.Commands;
using SamaniCrm.Application.SecuritySetting.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    public class SecuritySettingsController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public SecuritySettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetPasswordComplexity")]
        [ProducesResponseType(typeof(ApiResponse<PasswordComplexityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPasswordComplexity()
        {
            return ApiOk(await _mediator.Send(new GetPasswordComplexityQuery()));
        }


        #region Host security settings
        [HttpGet("GetSecuritySettings")]
        [Permission(AppPermissions.SecuritySetting_GetSetting)]
        [ProducesResponseType(typeof(ApiResponse<SecuritySettingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSecuritySettings()
        {
            return ApiOk(await _mediator.Send(new GetSecuritySettingsQuery()));
        }


        [HttpPost("UpdateSecuritySettings")]
        [Permission(AppPermissions.SecuritySetting_UpdateSetting)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSecuritySettings(UpdateSecuritySettingCommand input)
        {
            return ApiOk(await _mediator.Send(input));
        }
        #endregion

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
        public async Task<IActionResult> ChangeIsActiveExternalProvider(Guid id,bool isActive)
        {
            bool result = await _mediator.Send(new ChangeIsActiveExternalProviderCommand(id,isActive));
            return ApiOk(result);
        }

        #region User security setting

        [HttpGet("GetUserSecuritySettings")]
        [Permission(AppPermissions.SecuritySetting_GetUserSetting)]
        [ProducesResponseType(typeof(ApiResponse<UserSettingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSecuritySettings()
        {
            return ApiOk(await _mediator.Send(new GetUserSecuritySettingsQuery()));
        }


        [HttpPost("UpdateUserSecuritySettings")]
        [Permission(AppPermissions.SecuritySetting_UpdateUserSetting)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserSecuritySettings(UpdateUserSecuritySettingCommand input)
        {
            return ApiOk(await _mediator.Send(input));
        }

        #endregion
    }

}
