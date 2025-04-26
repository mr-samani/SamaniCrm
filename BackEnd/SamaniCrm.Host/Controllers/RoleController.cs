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
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles = "Admin, Management")]
    public class RoleController : ControllerBase
    {
        public readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        [ProducesDefaultResponseType(typeof(int))]

        public async Task<ActionResult<ApiResponse<int>>> CreateRoleAsync(RoleCreateCommand command)
        {
            return Ok(ApiResponse<int>.Ok(await _mediator.Send(command)));
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<List<RoleResponseDTO>>>> GetRoleAsync()
        {
            IList<RoleResponseDTO> result = await _mediator.Send(new GetRoleQuery());
            return Ok(ApiResponse<List<RoleResponseDTO>>.Ok((List<RoleResponseDTO>)result));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleResponseDTO>>> GetRoleByIdAsync(Guid id)
        {
            return Ok(ApiResponse<RoleResponseDTO>.Ok(await _mediator.Send(new GetRoleByIdQuery() { RoleId = id })));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteRoleAsync(Guid id)
        {
            var result= await _mediator.Send(new DeleteRoleCommand()
            {
                RoleId = id
            });
            return Ok(ApiResponse<int>.Ok(result));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult<ApiResponse<int>>> EditRole(string id, [FromBody] UpdateRoleCommand command)
        {
            if (id == command.Id.ToString())
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
