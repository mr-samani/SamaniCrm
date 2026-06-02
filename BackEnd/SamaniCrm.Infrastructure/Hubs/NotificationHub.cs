using Microsoft.AspNetCore.SignalR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Infrastructure.Connections;
using SamaniCrm.Infrastructure.Loging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SamaniCrm.Infrastructure.Hubs;

public class NotificationHub : Hub<INotificationHubService>
{
    private readonly IConnectionManager _connectionManager;
    private readonly IAppLogService _logger;

    public NotificationHub(
        IConnectionManager connectionManager,
        IAppLogService logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var deviceInfo = ExtractDeviceInfo();

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Connection {ConnectionId} without authenticated user", Context.ConnectionId);
            await base.OnConnectedAsync();
            return;
        }

        // Create connection info
        var connectionInfo = new ConnectionInfo
        {
            ConnectionId = Context.ConnectionId,
            UserId = userId,
            DeviceId = deviceInfo.DeviceId,
            DeviceType = deviceInfo.DeviceType,
            DeviceName = deviceInfo.DeviceName,
            Browser = deviceInfo.Browser,
            OperatingSystem = deviceInfo.OperatingSystem,
            IpAddress = GetClientIp(),
            UserAgent = Context.GetHttpContext()?.Request.Headers.UserAgent.ToString()
        };

        // Add connection to manager
        await _connectionManager.AddConnectionAsync(Context.ConnectionId, connectionInfo);

        // Create session for this device
        var session = await _connectionManager.CreateSessionAsync(
            userId,
            deviceInfo.DeviceId,
            deviceInfo.DeviceType,
            deviceInfo.DeviceName,
            GetClientIp());

        // Join user to their personal group for easy messaging
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        _logger.LogInformation(
            "User {UserId} connected from {DeviceType} ({Browser} on {OS}) - Connection: {ConnectionId}",
            userId, deviceInfo.DeviceType, deviceInfo.Browser, deviceInfo.OperatingSystem, Context.ConnectionId);

        // Notify about online status
        await Clients.Others.SendAsync("UserOnline", new
        {
            UserId = userId,
            DeviceType = deviceInfo.DeviceType,
            TotalConnections = _connectionManager.GetUserConnectionCount(userId)
        });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connection = _connectionManager.GetConnection(Context.ConnectionId);
        var userId = connection?.UserId ?? "Unknown";

        if (exception != null)
        {
            _logger.LogError(exception,
                "Client {ConnectionId} disconnected with error for user {UserId}",
                Context.ConnectionId, userId);
        }

        await _connectionManager.RemoveConnectionAsync(Context.ConnectionId);

        _logger.LogInformation(
            "User {UserId} disconnected (Connection: {ConnectionId})",
            userId, Context.ConnectionId);

        // Notify about offline status
        if (!string.IsNullOrEmpty(userId) && userId != "Unknown")
        {
            var remainingConnections = _connectionManager.GetUserConnectionCount(userId);

            if (remainingConnections == 0)
            {
                await Clients.Others.SendAsync("UserOffline", new { UserId = userId });
            }
            else
            {
                await Clients.Others.SendAsync("UserConnectionChanged", new
                {
                    UserId = userId,
                    RemainingConnections = remainingConnections
                });
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// ثبت فعالیت (برای heartbeat)
    /// </summary>
    public async Task Heartbeat()
    {
        await _connectionManager.UpdateLastActivityAsync(Context.ConnectionId);
        await Clients.Caller.SendAsync("HeartbeatAck", new { Timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// دریافت سشن‌های فعال
    /// </summary>
    public async Task GetActiveSessions()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var summary = _connectionManager.GetUserSessionSummary(userId);
        await Clients.Caller.SendAsync("ActiveSessions", summary);
    }

    /// <summary>
    /// بستن یک سشن خاص
    /// </summary>
    public async Task TerminateSession(string sessionId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var success = await _connectionManager.TerminateSessionAsync(userId, sessionId);
        await Clients.Caller.SendAsync("SessionTerminated", new { SessionId = sessionId, Success = success });
    }

    /// <summary>
    /// بستن سایر سشن‌ها
    /// </summary>
    public async Task TerminateOtherSessions(string currentSessionId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var count = await _connectionManager.TerminateOtherSessionsAsync(userId, currentSessionId);
        await Clients.Caller.SendAsync("OtherSessionsTerminated", new { Count = count });
    }

    #region Private Methods

    private string? GetUserId()
    {
        return Context?.User?.FindFirstValue(ClaimTypes.Sid);
    }

    private (string DeviceId, string DeviceType, string DeviceName, string Browser, string OperatingSystem) ExtractDeviceInfo()
    {
        var userAgent = Context.GetHttpContext()?.Request.Headers.UserAgent.ToString() ?? "";

        var (browser, os) = ParseUserAgent(userAgent);
        var deviceType = DetermineDeviceType(userAgent);
        var deviceId = GenerateDeviceId(userAgent);
        var deviceName = $"{browser} on {os}";

        return (deviceId, deviceType, deviceName, browser, os);
    }

    private (string Browser, string OS) ParseUserAgent(string userAgent)
    {
        string browser = "Unknown";
        string os = "Unknown";

        // Browser detection
        if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg"))
            browser = "Chrome";
        else if (userAgent.Contains("Firefox"))
            browser = "Firefox";
        else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
            browser = "Safari";
        else if (userAgent.Contains("Edg"))
            browser = "Edge";
        else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
            browser = "IE";

        // OS detection
        if (userAgent.Contains("Windows NT 10"))
            os = "Windows 10/11";
        else if (userAgent.Contains("Windows NT 6.3"))
            os = "Windows 8.1";
        else if (userAgent.Contains("Windows"))
            os = "Windows";
        else if (userAgent.Contains("Mac OS X"))
            os = "macOS";
        else if (userAgent.Contains("Linux"))
            os = "Linux";
        else if (userAgent.Contains("Android"))
            os = "Android";
        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            os = "iOS";

        return (browser, os);
    }

    private string DetermineDeviceType(string userAgent)
    {
        if (userAgent.Contains("Mobile") || userAgent.Contains("Android") ||
            userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            return "Mobile";
        if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
            return "Tablet";
        return "Desktop";
    }

    private string GenerateDeviceId(string userAgent)
    {
        // Generate a hash-based device ID from user agent
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userAgent));
        return Convert.ToBase64String(hash)[..16];
    }

    private string GetClientIp()
    {
        var context = Context.GetHttpContext();
        var ip = context?.Connection.RemoteIpAddress?.ToString();

        // Check for proxy headers
        var forwardedFor = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            ip = forwardedFor.Split(',')[0].Trim();
        }

        return ip ?? "Unknown";
    }

    #endregion
}
