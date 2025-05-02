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
using SamaniCrm.Api.Attributes;
using SamaniCrm.Core.AppPermissions;


namespace SamaniCrm.Api.Controllers
{
    [Authorize]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin, Management")]
    public class UserController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCurrentUser")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            return ApiOk(await _mediator.Send(new GetCurrentUserQuery()));
        }

        [HttpPost("Create")]
        [HasPermission(AppPermissions.UserManagement_Create)]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            return ApiOk(await _mediator.Send(command));
        }

        [HttpPost("GetAllUsers")]
        [HasPermission(AppPermissions.UserManagement_List)]
        [ProducesResponseType(typeof(PaginatedResult<UserResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUserAsync([FromBody] GetUserQuery request)
        {
            return ApiOk<PaginatedResult<UserResponseDTO>>(await _mediator.Send(request));
        }

        [HttpDelete("Delete/{userId}")]
        [HasPermission(AppPermissions.UserManagement_Delete)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand() { Id = userId });
            return ApiOk(result);
        }

        [HttpGet("GetUserDetails/{userId}")]
        [HasPermission(AppPermissions.UserManagement_List)]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var result = await _mediator.Send(new GetUserDetailsQuery() { UserId = userId });
            return ApiOk(result);
        }

        [HttpGet("GetUserDetailsByUserName/{userName}")]
        [HasPermission(AppPermissions.UserManagement_List)]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<IActionResult> GetUserDetailsByUserName(string userName)
        {
            var result = await _mediator.Send(new GetUserDetailsByUserNameQuery() { UserName = userName });
            return ApiOk(result);
        }

        [HttpPost("AssignRoles")]
        [HasPermission(AppPermissions.UserManagement_AssignRole)]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> AssignRoles(AssignUsersRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return ApiOk(result);
        }

        [HttpPut("EditUserRoles")]
        [HasPermission(AppPermissions.UserManagement_AssignRole)]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> EditUserRoles(UpdateUserRolesCommand command)
        {
            var result = await _mediator.Send(command);
            return ApiOk(result);
        }




        [HttpPut("EditUserProfile/{id}")]
        [HasPermission(AppPermissions.UserManagement_EditUserProfile)]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> EditUserProfile(string id, [FromBody] EditUserProfileCommand command)
        {
            if (id == command.Id)
            {
                var result = await _mediator.Send(command);
                return ApiOk(result);
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
