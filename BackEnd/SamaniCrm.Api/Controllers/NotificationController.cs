using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.NotificationManager.Commands;
using SamaniCrm.Application.NotificationManager.Interfaces;
using SamaniCrm.Application.NotificationManager.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Host.Models;
using SamaniCrm.Infrastructure.Connections;

namespace SamaniCrm.Api.Controllers;

[Authorize]
public class NotificationController : ApiBaseController
{
    private readonly IMediator _mediator;

    private readonly INotificationSender _notificationSender;
    private readonly IConnectionManager _connectionManager;
    public NotificationController(IMediator mediator, INotificationSender notificationSender, IConnectionManager connectionManager)
    {
        _mediator = mediator;
        _notificationSender = notificationSender;
        _connectionManager = connectionManager;
    }



    [HttpPost("SendMessageToUser")]
    [Permission(AppPermissions.Notification.SendMessageToUser)]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessageToUser(SendNotificationCommand request)
    {
        Unit result = await _mediator.Send(request);
        return ApiOk(result);

    }

    [HttpPost("BroadCastMessageToAllUsers")]
    [Permission(AppPermissions.Notification.BroadCastMessageToAll)]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BroadCastMessageToAllUsers(BroadCastNotificationsCommand request)
    {
        long result = await _mediator.Send(request);
        return ApiOk(result);

    }



    [HttpPost("GetAllNotifications")]
    [Permission(AppPermissions.Notification.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllNotifications([FromBody] GetAllNotificationQuery command)
    {
        PaginatedResult<NotificationDto> result = await _mediator.Send(command);
        return ApiOk(result);
    }



    [HttpPost("DeleteNotification")]
    [Permission(AppPermissions.Notification.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteNotification([FromBody] DeleteNotificationCommand command)
    {
        bool result = await _mediator.Send(command);
        return ApiOk(result);
    }


    [HttpPost("MarkAsRead")]
    [Permission(AppPermissions.Notification.List)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadCommand command)
    {
        bool result = await _mediator.Send(command);
        return ApiOk(result);
    }

    [HttpPost("MarkAllAsRead")]
    [Permission(AppPermissions.Notification.MarkAllAsRead)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        bool result = await _mediator.Send(new MarkAllAsReadCommand());
        return ApiOk(result);
    }

    [HttpGet("GetNotificationInfo")]
    [Permission(AppPermissions.Notification.List)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotificationInfo(Guid Id)
    {
        NotificationDto result = await _mediator.Send(new GetNotificationInfoQuery(Id));
        return ApiOk(result);
    }


    [HttpGet("GetLastUnReadNotifications")]
    [Permission(AppPermissions.Notification.List)]
    [ProducesResponseType(typeof(ApiResponse<UnReadNotificationListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLastUnReadNotifications()
    {
        UnReadNotificationListDto result = await _mediator.Send(new GetLastUnReadNotificationsQuery());
        return ApiOk(result);
    }


    /***************************************************************************/
    /// <summary>
    /// ارسال نوتیفیکیشن به یک کاربر
    /// </summary>
    [HttpPost("send/{userId}")]
    public async Task<IActionResult> SendToUser(string userId, [FromBody] NotificationPayload payload)
    {
        await _notificationSender.SendToUserAsync(userId, payload.Method, payload.Data);
        return Ok();
    }

    /// <summary>
    /// ارسال به دستگاه خاص
    /// </summary>
    [HttpPost("send/{userId}/device/{deviceId}")]
    public async Task<IActionResult> SendToUserDevice(string userId, string deviceId,
        [FromBody] NotificationPayload payload)
    {
        await _notificationSender.SendToUserDeviceAsync(userId, deviceId, payload.Method, payload.Data);
        return Ok();
    }

    /// <summary>
    /// دریافت سشن‌های فعال کاربر
    /// </summary>
    [HttpGet("sessions/{userId}")]
    public IActionResult GetUserSessions(string userId)
    {
        var summary = _connectionManager.GetUserSessionSummary(userId);
        return Ok(summary);
    }

    /// <summary>
    /// بستن سشن‌های دیگر کاربر
    /// </summary>
    [HttpPost("sessions/{userId}/terminate-others/{currentSessionId}")]
    public async Task<IActionResult> TerminateOtherSessions(string userId, string currentSessionId)
    {
        var count = await _connectionManager.TerminateOtherSessionsAsync(userId, currentSessionId);
        return Ok(new { TerminatedCount = count });
    }

    /// <summary>
    /// آمار کلی
    /// </summary>
    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        var stats = _connectionManager.GetStatistics();
        return Ok(stats);
    }

    /// <summary>
    /// بررسی آنلاین بودن کاربر
    /// </summary>
    [HttpGet("online/{userId}")]
    public IActionResult IsUserOnline(string userId)
    {
        var isOnline = _connectionManager.IsUserOnline(userId);
        var connectionCount = _connectionManager.GetUserConnectionCount(userId);

        return Ok(new { UserId = userId, IsOnline = isOnline, ConnectionCount = connectionCount });
    }
    public class NotificationPayload
    {
        public string Method { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
