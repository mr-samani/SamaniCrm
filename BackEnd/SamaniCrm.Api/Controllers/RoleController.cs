using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.Role;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Host.Models;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Core.AppPermissions;


namespace SamaniCrm.Api.Controllers
{

    [Authorize]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin, Management")]
    public class RoleController : ApiBaseController
    {
        public readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        [HasPermission(AppPermissions.RoleManagement_Create)]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> CreateRoleAsync(RoleCreateCommand command)
        {
            return ApiOk<int>(await _mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [HasPermission(AppPermissions.RoleManagement_List)]
        [ProducesResponseType(typeof(List<RoleResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var result = await _mediator.Send(new GetRoleQuery());
            return ApiOk<IList<RoleResponseDTO>>(result);
        }


        [HttpGet("{id}")]
        [HasPermission(AppPermissions.RoleManagement_List)]
        [ProducesResponseType(typeof(RoleResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleByIdAsync(Guid id)
        {
            return ApiOk(await _mediator.Send(new GetRoleByIdQuery() { RoleId = id }));
        }

        [HttpDelete("Delete/{id}")]
        [HasPermission(AppPermissions.RoleManagement_Delete)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRoleAsync(Guid id)
        {
            int result = await _mediator.Send(new DeleteRoleCommand()
            {
                RoleId = id
            });
            return ApiOk<int>(result);
        }

        [HttpPut("Edit/{id}")]
        [HasPermission(AppPermissions.RoleManagement_Edit)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> EditRole(string id, [FromBody] UpdateRoleCommand command)
        {
            if (id == command.Id.ToString())
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
