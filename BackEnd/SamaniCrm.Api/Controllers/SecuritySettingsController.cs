using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.SecuritySetting.Commands;
using SamaniCrm.Application.SecuritySetting.Queries;
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
        [ProducesResponseType(typeof(ApiResponse<PasswordComplexityDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPasswordComplexity()
        {
            return ApiOk(await _mediator.Send(new GetPasswordComplexityQuery()));
        }



        [HttpGet("GetSecuritySettings")]
        [ProducesResponseType(typeof(ApiResponse<SecuritySettingDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSecuritySettings()
        {
            return ApiOk(await _mediator.Send(new GetSecuritySettingsQuery()));
        }


        [HttpPost("UpdateSecuritySettings")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSecuritySettings(UpdateSecuritySettingCommand input)
        {
            return ApiOk(await _mediator.Send(input));
        }


    }

}
