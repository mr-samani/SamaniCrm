using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.User.Queries;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    public class InitialController : ApiBaseController
    {
        public readonly IMediator _mediator;

        public InitialController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("initialApp")]
        [ProducesResponseType(typeof(InitialAppDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> initialApp()
        {
            return ApiOk<InitialAppDTO>(await _mediator.Send(new InitialAppQuery()));

        }
    }
}
