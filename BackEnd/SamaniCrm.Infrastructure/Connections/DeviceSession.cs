using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Connections;

public interface IDeviceSession
{
    string SessionId { get; }
    string UserId { get; }
    string DeviceId { get; }
    string DeviceType { get; }
    string DeviceName { get; }
    string IpAddress { get; }
    DateTime LoginAt { get; }
    DateTime LastActivity { get; }
    DateTime? ExpiresAt { get; }
    bool IsCurrent { get; }
    List<string> ConnectionIds { get; }
}

public class DeviceSession : IDeviceSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = "Unknown";
    public string DeviceName { get; set; } = "Unknown";
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsCurrent { get; set; }
    public List<string> ConnectionIds { get; set; } = new();
}