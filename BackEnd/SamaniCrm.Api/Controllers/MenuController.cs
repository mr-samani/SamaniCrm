using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Localize.Queries;
using SamaniCrm.Application.Menu.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class MenuController : ApiBaseController
    {

        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllMenus")]
       // [Permission(AppPermissions.MenuManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMenus()
        {
            var result = await _mediator.Send(new GetAllMenuItemsQuery());
            return ApiOk<List<MenuDTO>>(result);
        }

    }
}
