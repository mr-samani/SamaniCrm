using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Api.Controllers;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;
using SamaniCrm.Infrastructure.Identity;
using SamaniCrm.Infrastructure.Identity.Migrations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Host.Controllers
{

    public class AccountController : ApiBaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly ITwoFactorService _twoFactorService;

        public AccountController(
            IConfiguration configuration, IMediator mediator, ITwoFactorService twoFactorService)
        {
            _configuration = configuration;
            _mediator = mediator;
            _twoFactorService = twoFactorService;
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            LoginResult result = await _mediator.Send(command);
            return ApiOk<LoginResult>(result);
        }

        [HttpPost("loginTwoFactor")]
        [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginTwoFactor(TwoFactorLoginCommand command)
        {
            LoginResult result = await _mediator.Send(command);
            return ApiOk<LoginResult>(result);
        }


        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return ApiOk<TokenResponseDto>(result);
        }

        [HttpPost("revoke")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenCommand command)
        {
            var success = await _mediator.Send(command);
            //if (!success)
            //    return BadRequest(ApiResponse<string>.Fail(new List<ApiError> { new() { Message = "Token invalid or already revoked" } }));

            return ApiOk<string>("");
        }




        [HttpPost("generate2FaRequest")]
        [Permission(AppPermissions.SecuritySetting_TwoFactorApp)]
        [ProducesResponseType(typeof(ApiResponse<GenerateTwoFactorCodeDto>), StatusCodes.Status200OK)]
        public IActionResult generate2FaRequestGenerate()
        {
            var result = _twoFactorService.GenerateSetupCode("SamaniCrm");
            return ApiOk(result);
        }

        [HttpPost("Verify2FaApp")]
        [Permission(AppPermissions.SecuritySetting_TwoFactorApp)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Verify2FaApp([FromBody] Verify2FARequest req)
        {
            var result = await _twoFactorService.Save2FaVerifyCodeAsync(req.Secret, req.Code);
            return ApiOk(result);
        }


        [HttpGet("GetExternalProviders")]
        [ProducesResponseType(typeof(ApiResponse<List<ExternalProviderDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExternalProviders()
        {
            List<ExternalProviderDto> result = await _mediator.Send(new GetExternalProvidersQuery(true));
            return ApiOk(result);
        }



        //[HttpGet("external-login")]
        //public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl = "/")
        //{
        //    var redirectUrl = await _mediator.Send(new ExternalLoginCommand(provider, returnUrl));
        //    return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, provider);
        //}

        [HttpPost("ExternalLoginCallback")]
        [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExternalLoginCallback(ExternalLoginCallbackCommand request)
        {
            LoginResult result = await _mediator.Send(request);

            return ApiOk<LoginResult>(result);
        }






    }





    public class Verify2FARequest
    {
        public string Secret { get; set; } = "";
        public string Code { get; set; } = "";
    }

}