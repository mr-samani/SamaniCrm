using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.Role;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Role.Commands;
using SamaniCrm.Host.Models;


namespace SamaniCrm.Api.Controllers
{

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
        [ProducesDefaultResponseType(typeof(int))]

        public async Task<IActionResult> CreateRoleAsync(RoleCreateCommand command)
        {
            return ApiOk<int>(await _mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<RoleResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleAsync()
        {
            var result = await _mediator.Send(new GetRoleQuery());
            return ApiOk<IList<RoleResponseDTO>>(result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleByIdAsync(Guid id)
        {
            return ApiOk(await _mediator.Send(new GetRoleByIdQuery() { RoleId = id }));
        }

        [HttpDelete("Delete/{id}")]
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
