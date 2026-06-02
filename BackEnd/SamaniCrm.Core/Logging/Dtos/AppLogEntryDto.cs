using Microsoft.Extensions.Logging;

namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class AppLogEntryDto
{
    public long Id { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExceptionDetails { get; set; }
    public string? Source { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public Dictionary<string, object>? ExtraData { get; set; }

    // محاسبه Duration در میلی‌ثانیه
    public long? Duration { get; set; }
}
