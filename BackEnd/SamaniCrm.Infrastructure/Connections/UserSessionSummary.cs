namespace SamaniCrm.Infrastructure.Connections;

public class UserSessionSummary
{
    public string UserId { get; set; } = string.Empty;
    public int TotalConnections { get; set; }
    public int ActiveSessions { get; set; }
    public List<DeviceSessionInfo> Sessions { get; set; } = new();
}
public class DeviceSessionInfo
{
    public string SessionId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsCurrent { get; set; }
    public int ConnectionCount { get; set; }
}