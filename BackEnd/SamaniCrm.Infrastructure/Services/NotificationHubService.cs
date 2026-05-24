using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(Guid userId, NotificationDto message)
    {
        var userIdString = userId.ToString();
        var userClients = _hubContext.Clients.User(userIdString);

        // لاگ کن چند تا connection برای این user پیدا شده
        Console.WriteLine($"User clients type: {userClients.GetType().Name.ToString()}");
        await userClients.SendAsync("ReceiveNotification", message);

        Console.WriteLine("Notification sent successfully");

     }

    public async Task SendMessage(Guid userId, string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"{userId}: {message}");
    }

}
