using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Infrastructure.Loging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SamaniCrm.Infrastructure.Hubs;

public class NotificationHub : Hub<INotificationHubService>
{
    private readonly ILogService _logger;

    public NotificationHub(ILogService logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    { 
        string? userId = Context?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context?.User?.FindFirstValue("sub")
                ?? Context?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        Console.WriteLine($"User {userId} connected to SignalR hub.");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception,
                "Client disconnected with error: {ConnectionId}",
                Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
   

}
