using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.InitialApp.Queries; 
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    public class CommonController : ApiBaseController
    {
        public readonly IMediator _mediator;

        public CommonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("InitialApp")]
        [ProducesResponseType(typeof(ApiResponse<InitialAppDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> InitialApp()
        {
            return ApiOk<InitialAppDTO>(await _mediator.Send(new InitialAppQuery()));

        }

    }
}
