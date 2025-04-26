using SamaniCrm.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Host.Models;


namespace SamaniCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin, Management")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult<ApiResponse<int>>> CreateUser(CreateUserCommand command)
        {
            return Ok(ApiResponse<int>.Ok(await _mediator.Send(command)));
        }

        [HttpGet("GetAll")]
        [ProducesDefaultResponseType(typeof(List<UserResponseDTO>))]
        public async Task<ActionResult<ApiResponse<List<UserResponseDTO>>>> GetAllUserAsync()
        {
            return Ok(ApiResponse<List<UserResponseDTO>>.Ok(await _mediator.Send(new GetUserQuery())));
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

        [HttpGet("GetAllUserDetails")]
        public async Task<ActionResult<ApiResponse<List<UserDetailsResponseDTO>>>> GetAllUserDetails()
        {
            List<UserDetailsResponseDTO> result = await _mediator.Send(new GetAllUsersDetailsQuery());
            return Ok(ApiResponse<List<UserDetailsResponseDTO>>.Ok(result));
        }


        [HttpPut("EditUserProfile/{id}")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult<ApiResponse<int>>> EditUserProfile(string id, [FromBody] EditUserProfileCommand command)
        {
            if (id == command.Id)
            {
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<int>.Ok( result));
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
