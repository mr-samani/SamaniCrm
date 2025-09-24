using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.NotificationManager.Commands;
using SamaniCrm.Application.NotificationManager.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    public class NotificationController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost("SendMessageToUser")]
        [Permission(AppPermissions.Notification_SendMessageToUser)]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessageToUser(SendNotificationCommand request)
        {
            Unit result = await _mediator.Send(request);
            return ApiOk(result);

        }

        [HttpPost("BroadCastMessageToAllUsers")]
        [Permission(AppPermissions.Notification_BroadCastMessageToAll)]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BroadCastMessageToAllUsers(BroadCastNotificationsCommand request)
        {
            long result = await _mediator.Send(request);
            return ApiOk(result);

        }



        [HttpPost("GetAllNotifications")]
        [Permission(AppPermissions.Notification_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<NotificationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllNotifications([FromBody] GetAllNotificationQuery command)
        {
            PaginatedResult<NotificationDto> result = await _mediator.Send(command);
            return ApiOk(result);
        }



        [HttpPost("DeleteNotification")]
        [Permission(AppPermissions.Notification_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteNotification([FromBody] DeleteNotificationCommand command)
        {
            bool result = await _mediator.Send(command);
            return ApiOk(result);
        }


        [HttpPost("MarkAsRead")]
        [Permission(AppPermissions.Notification_List)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadCommand command)
        {
            bool result = await _mediator.Send(command);
            return ApiOk(result);
        }

        [HttpPost("MarkAllAsRead")]
        [Permission(AppPermissions.Notification_MarkAllAsRead)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            bool result = await _mediator.Send(new MarkAllAsReadCommand());
            return ApiOk(result);
        }

        [HttpGet("GetNotificationInfo")]
        [Permission(AppPermissions.Notification_List)]
        [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotificationInfo(Guid Id)
        {
            NotificationDto result = await _mediator.Send(new GetNotificationInfoQuery(Id));
            return ApiOk(result);
        }


        [HttpGet("GetLastUnReadNotifications")]
        [Permission(AppPermissions.Notification_List)]
        [ProducesResponseType(typeof(ApiResponse<List<NotificationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLastUnReadNotifications()
        {
            List<NotificationDto> result = await _mediator.Send(new GetLastUnReadNotificationsQuery());
            return ApiOk(result);
        }
    }
}
