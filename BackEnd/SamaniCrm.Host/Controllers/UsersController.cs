using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Users.Queries;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Retrieve paginated, sortable, filterable list of users.</summary>
        [HttpPost("GetUsers")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers([FromBody] UserListQuery query)
        {
            var result = await _mediator.Send(query);

            // Build pagination metadata
            var meta = new Meta
            {
                TotalCount = result.TotalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

       
            return Ok(ApiResponse<List<UserDto>>.Ok(result.Items, meta));
        }
    }
}
