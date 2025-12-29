using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Application.Queries.User;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Application.User.Commands;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;


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
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            return ApiOk(await _mediator.Send(new GetCurrentUserQuery()));
        }

        [HttpPost("CreateUser")]
        [Permission(AppPermissions.UserManagement_Create)]
        [ProducesDefaultResponseType(typeof(ApiResponse<int>))]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            return ApiOk(await _mediator.Send(command));
        }

        [HttpPost("UpdateUser")]
        [Permission(AppPermissions.UserManagement_Edit)]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateUser([FromBody] EditUserCommand command)
        {
            var result = await _mediator.Send(command);
            return ApiOk(result);
        }


        [HttpPost("GetAllUsers")]
        [Permission(AppPermissions.UserManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUserAsync([FromBody] GetUserQuery request)
        {
            return ApiOk<PaginatedResult<UserDTO>>(await _mediator.Send(request));
        }

        [HttpPost("Delete/{userId}")]
        [Permission(AppPermissions.UserManagement_Delete)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand() { Id = userId });
            return ApiOk(result);
        }




        [HttpGet("GetUserDetails/{userId}")]
        [Permission(AppPermissions.UserManagement_List)]
        [ProducesDefaultResponseType(typeof(ApiResponse<UserDTO>))]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var result = await _mediator.Send(new GetUserDetailsQuery() { UserId = userId });
            return ApiOk(result);
        }

        [HttpGet("GetUserDetailsByUserName/{userName}")]
        [Permission(AppPermissions.UserManagement_List)]
        [ProducesDefaultResponseType(typeof(ApiResponse<UserDTO>))]
        public async Task<IActionResult> GetUserDetailsByUserName(string userName)
        {
            var result = await _mediator.Send(new GetUserDetailsByUserNameQuery() { UserName = userName });
            return ApiOk(result);
        }

        //[HttpPost("AssignRoles")]
        //[Permission(AppPermissions.UserManagement_AssignRole)]
        //[ProducesDefaultResponseType(typeof(ApiResponse<int>))]
        //public async Task<IActionResult> AssignRoles(AssignUsersRoleCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return ApiOk(result);
        //}

        //[HttpPut("EditUserRoles")]
        //[Permission(AppPermissions.UserManagement_AssignRole)]
        //[ProducesDefaultResponseType(typeof(ApiResponse<int>))]
        //public async Task<IActionResult> EditUserRoles(UpdateUserRolesCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return ApiOk(result);
        //}



        [HttpGet("updateCurrentuserLanguage")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> updateCurrentuserLanguage(string culture)
        {
            var result = await _mediator.Send(new ChangeUserLanguageCommand(culture));
            return ApiOk(result);
        }


        [HttpGet("GetAutoCompleteUser")]
        [Permission(AppPermissions.UserManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<List<AutoCompleteDto<Guid>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAutoCompleteUser(string? filter, CancellationToken cancellationToken)
        {
            List<AutoCompleteDto<Guid>> result = await _mediator.Send(new GetAutoCompleteUserQuery(filter), cancellationToken);
            return ApiOk(result);
        }



    }
}
