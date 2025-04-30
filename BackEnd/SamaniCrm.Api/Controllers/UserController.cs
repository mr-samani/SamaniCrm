using SamaniCrm.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Host.Models;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.User.Queries;


namespace SamaniCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin, Management")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<ApiResponse<UserResponseDTO>>> GetCurrentUser()
        {
            return Ok(ApiResponse<UserResponseDTO>.Ok(await _mediator.Send(new GetCurrentUserQuery())));
        }

        [HttpPost("Create")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult<ApiResponse<int>>> CreateUser(CreateUserCommand command)
        {
            return Ok(ApiResponse<int>.Ok(await _mediator.Send(command)));
        }

        [HttpPost("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<UserResponseDTO>>>> GetAllUserAsync([FromBody] GetUserQuery request)
        {
            return Ok(ApiResponse<PaginatedResult<UserResponseDTO>>.Ok(await _mediator.Send(request)));
        }

        [HttpDelete("Delete/{userId}")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult<ApiResponse<int>>> DeleteUser(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand() { Id = userId });
            return Ok(ApiResponse<int>.Ok(result));
        }

        [HttpGet("GetUserDetails/{userId}")]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<ActionResult<ApiResponse<UserDetailsResponseDTO>>> GetUserDetails(Guid userId)
        {
            var result = await _mediator.Send(new GetUserDetailsQuery() { UserId = userId });
            return Ok(ApiResponse<UserDetailsResponseDTO>.Ok(result));
        }

        [HttpGet("GetUserDetailsByUserName/{userName}")]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<ActionResult<ApiResponse<UserDetailsResponseDTO>>> GetUserDetailsByUserName(string userName)
        {
            var result = await _mediator.Send(new GetUserDetailsByUserNameQuery() { UserName = userName });
            return Ok(ApiResponse<UserDetailsResponseDTO>.Ok(result));
        }

        [HttpPost("AssignRoles")]
        [ProducesDefaultResponseType(typeof(int))]

        public async Task<ActionResult<ApiResponse<int>>> AssignRoles(AssignUsersRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<int>.Ok(result));
        }

        [HttpPut("EditUserRoles")]
        [ProducesDefaultResponseType(typeof(int))]

        public async Task<ActionResult<ApiResponse<int>>> EditUserRoles(UpdateUserRolesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<int>.Ok(result));
        }




        [HttpPut("EditUserProfile/{id}")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult<ApiResponse<int>>> EditUserProfile(string id, [FromBody] EditUserProfileCommand command)
        {
            if (id == command.Id)
            {
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<int>.Ok(result));
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
