using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class LastLoginDto
{
    public SecurityEventType EventType { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
