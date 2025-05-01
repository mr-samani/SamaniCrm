using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitialController : ControllerBase
    {
        public readonly IMediator _mediator;

        public InitialController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("initialApp")]
        public async Task<ActionResult<ApiResponse<InitialAppDTO>>> initialApp()
        {
            return Ok(ApiResponse<InitialAppDTO>.Ok(await _mediator.Send(new InitialAppQuery())));

        }
    }
}
