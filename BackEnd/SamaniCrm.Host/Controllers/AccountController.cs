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

namespace SamaniCrm.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
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
        public async Task<ActionResult<ApiResponse<LoginResult>>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<LoginResult>.Ok(result));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<TokenResponseDto>.Ok(result));
        }

        [HttpPost("revoke")]
        public async Task<ActionResult<ApiResponse<string>>> Revoke([FromBody] RevokeRefreshTokenCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success)
                return BadRequest(ApiResponse<string>.Fail(new List<ApiError> { new() { Message = "Token invalid or already revoked" } }));

            return Ok(ApiResponse<string>.Ok(data: ""));
        }

    }
}