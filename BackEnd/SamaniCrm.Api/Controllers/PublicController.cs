using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Menu.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    [AllowAnonymous]
    public class PublicController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public PublicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllActiveMenus")]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActiveMenus(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllActiveMenusQuery(), cancellationToken);
            return ApiOk<List<MenuDTO>>(result);
        }
    }
}
