using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.NotificationManager.Interfaces;
using SamaniCrm.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Connections;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub, INotificationHubService> _hubContext;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        IHubContext<NotificationHub, INotificationHubService> hubContext,
        IConnectionManager connectionManager,
        ILogger<NotificationSender> logger)
    {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task SendToUserAsync(string userId, string method, object? message)
    {
        // روش 1: از طریق گروه کاربر (ساده و سریع)
        await _hubContext.Clients.Group($"user:{userId}").SendAsync(method, message);

        _logger.LogDebug("Sent {Method} to user {UserId}", method, userId);
    }

    public async Task SendToUserDeviceAsync(string userId, string deviceId, string method, object? message)
    {
        var connections = _connectionManager.GetUserConnections(userId);

        foreach (var conn in connections)
        {
            if (conn.DeviceId == deviceId)
            {
                await _hubContext.Clients.Client(conn.ConnectionId).SendAsync(method, message);
                _logger.LogDebug("Sent {Method} to user {UserId} on device {DeviceId}",
                    method, userId, deviceId);
                return;
            }
        }

        _logger.LogWarning("No connection found for user {UserId} on device {DeviceId}", userId, deviceId);
    }

    public async Task SendToUsersAsync(IEnumerable<string> userIds, string method, object? message)
    {
        foreach (var userId in userIds)
        {
            await SendToUserAsync(userId, method, message);
        }
    }

    public async Task SendToGroupAsync(string groupName, string method, object? message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(method, message);
        _logger.LogDebug("Sent {Method} to group {GroupName}", method, groupName);
    }

    public async Task SendToAllAsync(string method, object? message)
    {
        await _hubContext.Clients.All.SendAsync(method, message);
        _logger.LogDebug("Sent {Method} to all clients", method);
    }

    public async Task SendToOnlineUsersAsync(string method, object? message)
    {
        var allConnections = _connectionManager.GetAllConnections();
        var uniqueUserIds = allConnections.Select(c => c.UserId).Distinct();

        foreach (var userId in uniqueUserIds)
        {
            await SendToUserAsync(userId, method, message);
        }
    }
}