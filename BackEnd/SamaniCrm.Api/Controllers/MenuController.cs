using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Localize.Queries;
using SamaniCrm.Application.Menu.Commands;
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
        [Permission(AppPermissions.MenuManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMenus()
        {
            var result = await _mediator.Send(new GetAllMenuItemsQuery());
            return ApiOk<List<MenuDTO>>(result);
        }


        [HttpGet("GetForEdit")]
        [Permission(AppPermissions.MenuManagement_List)]
        [ProducesResponseType(typeof(ApiResponse<MenuDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForEdit(Guid id)
        {
            var result = await _mediator.Send(new GetMenuForEditQuery(id));
            return ApiOk<MenuDTO>(result);
        }


        [HttpPost("CreateOrUpdate")]
        [Permission(AppPermissions.MenuManagement_Create)]
        [Permission(AppPermissions.MenuManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdate(CreateOrEditMenuCommand request)
        {
            var result = await _mediator.Send(request);
            return ApiOk<Guid>(result);
        }

        [HttpPost("DeleteMenu")]
        [Permission(AppPermissions.MenuManagement_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMenu(DeleteMenuCommand request)
        {
            var result = await _mediator.Send(request);
            return ApiOk<bool>(result);
        }

        [HttpPost("ReOrderMenu")]
        [Permission(AppPermissions.MenuManagement_ReOrder)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReOrderMenu(ReorderMenuCommand request)
        {
            var result = await _mediator.Send(request);
            return ApiOk<bool>(result);
        }

        [HttpPost("ActiveOrDeactive")]
        [Permission(AppPermissions.MenuManagement_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActiveOrDeactive(ChangeActiveMenuCommand request)
        {
            var result = await _mediator.Send(request);
            return ApiOk<bool>(result);
        }

    }
}
