using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Connections;

public interface IConnectionInfo
{
    string ConnectionId { get; }
    string UserId { get; }
    string DeviceId { get; }
    string DeviceType { get; }
    string DeviceName { get; }
    string Browser { get; }
    string OperatingSystem { get; }
    string IpAddress { get; }
    string? UserAgent { get; }
    DateTime ConnectedAt { get; }
    DateTime LastActivity { get; }
    bool IsActive { get; }
    Dictionary<string, string> Metadata { get; }
}
public class ConnectionInfo : IConnectionInfo
{
    public string ConnectionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = "Unknown";
    public string DeviceName { get; set; } = "Unknown";
    public string Browser { get; set; } = "Unknown";
    public string OperatingSystem { get; set; } = "Unknown";
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public Dictionary<string, string> Metadata { get; set; } = new();
}