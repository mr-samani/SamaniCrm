using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Infrastructure.Services.TenantService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SamaniCrm.Infrastructure.Hubs;

// [Authorize]
public class ProvisioningHub : Hub<IProvisioningClient>
{
    private readonly ILogger<ProvisioningHub> _logger;

    public ProvisioningHub(ILogger<ProvisioningHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinTenantGroup(string tenantSlug)
    {
        var groupName = $"tenant-{tenantSlug}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation(
            "Client {ConnectionId} joined tenant group {Group}",
            Context.ConnectionId, groupName);
    }

    public async Task LeaveTenantGroup(string tenantSlug)
    {
        var groupName = $"tenant-{tenantSlug}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation(
            "Client {ConnectionId} left tenant group {Group}",
            Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        string? userId = Context?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? Context?.User?.FindFirstValue("sub")
               ?? Context?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        Console.WriteLine($"User {userId} connected to SignalR hub.");
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
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

public interface IProvisioningClient
{
    Task OnProgress(ProvisioningNotification notification);
    Task OnCompleted(ProvisioningNotification notification);
    Task OnError(ProvisioningNotification notification);
}

public class TenantUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("sub")?.Value
            ?? connection.ConnectionId;
    }
}