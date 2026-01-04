using MediatR;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Api.Controllers;
using SamaniCrm.Application.DashboardManager;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Host.Controllers
{
    public class DasboardController : ApiBaseController
    {

        private readonly IMediator _mediator;

        public DasboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllDashboards")]
        [Permission(AppPermissions.Dashboard_List)]
        [ProducesResponseType(typeof(ApiResponse<List<DashboardDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDashboards()
        {
            List<DashboardDto> result = await _mediator.Send(new GetAllDashboardsQuery());
            return ApiOk<List<DashboardDto>>(result);
        }

        [HttpPost("CreateOrUpdateDashboard")]
        [Permission(AppPermissions.Dashboard_Edit)]
        [Permission(AppPermissions.Dashboard_Create)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateDashboard(CreateOrUpdateDashboardCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        [HttpPost("DeleteDashboard")]
        [Permission(AppPermissions.Dashboard_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDashboard(DeleteDashboardCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


        [HttpGet("GetAllDashboardItems")]
        [Permission(AppPermissions.Dashboard_List)]
        [ProducesResponseType(typeof(ApiResponse<List<DashboardItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDashboardItems(Guid dashboardId)
        {
            List<DashboardItemDto> result = await _mediator.Send(new GetAllDashboardItemsQuery(dashboardId));
            return ApiOk<List<DashboardItemDto>>(result);
        }

        [HttpPost("CreateOrUpdateDashboardItem")]
        [Permission(AppPermissions.Dashboard_Item_Create)]
        [Permission(AppPermissions.Dashboard_Item_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateDashboardItem(CreateOrUpdateDashboardItemCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }
        
        [HttpPost("UpdateDashboardItems")]
        [Permission(AppPermissions.Dashboard_Item_Edit)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDashboardItems(UpdateDashboardItemsCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }



        [HttpPost("DeleteDashboardItem")]
        [Permission(AppPermissions.Dashboard_Item_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDashboardItem(DeleteDashboardItemCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


    }
}