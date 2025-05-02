using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SamaniCrm.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SamaniCrm.Host.Models;
using MediatR;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Api.Controllers;

namespace SamaniCrm.Host.Controllers
{

    public class AccountController : ApiBaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public AccountController(
            IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
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

    }
}