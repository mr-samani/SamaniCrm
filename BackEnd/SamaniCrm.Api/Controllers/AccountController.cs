using MediatR;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Api.Controllers;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Host.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SamaniCrm.Host.Controllers;


public class AccountController : ApiBaseController
{

    private readonly IMediator _mediator;
    private readonly ITwoFactorService _twoFactorService;

    public AccountController(IMediator mediator, ITwoFactorService twoFactorService)
    {
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
    [HttpPost("Logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return ApiOk(true);
    }

    [HttpPost("DelegateUser")]
    [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
    [Permission(AppPermissions.TenantManagement.DelegateUser)]
    public async Task<IActionResult> DelegateUser(DelegateUserCommand command)
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


    [HttpPost("generate2FaRequest")]
    [Permission(AppPermissions.SecuritySetting.TwoFactorApp)]
    [ProducesResponseType(typeof(ApiResponse<GenerateTwoFactorCodeDto>), StatusCodes.Status200OK)]
    public IActionResult generate2FaRequestGenerate()
    {
        var result = _twoFactorService.GenerateSetupCode("SamaniCrm");
        return ApiOk(result);
    }

    [HttpPost("Verify2FaApp")]
    [Permission(AppPermissions.SecuritySetting.TwoFactorApp)]
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