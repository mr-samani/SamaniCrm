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
using SamaniCrm.Application.Auth;

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
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                var result =await _mediator.Send(command);
                return Ok(new ApiResponse<LoginResult>(true, "OK", StatusCodes.Status200OK, result));
            }
            catch (InvalidLoginException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<LoginResult>(false, ex.Message, StatusCodes.Status500InternalServerError));
            }


        }
    }

}