using Microsoft.Extensions.Logging;
using SamaniCrm.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities;

/// <summary>
/// هر لاگ ذخیره‌شده
/// </summary>
public class AppLogEntry:IMayHaveTenant
{
    public long Id { get; set; }
    public Guid? TenantId { get; set; }

    // اطلاعات لاگ
    public LogLevel Level { get; set; }
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
    [MaxLength(10000)]
    public string? ExceptionDetails { get; set; }

    // منبع لاگ
    [MaxLength(200)]
    public string? Source { get; set; }        // نام کلاس/سرویس
    [MaxLength(200)]
    public string? ActionName { get; set; }    // نام اکشن
    [MaxLength(200)]
    public string? ControllerName { get; set; }
    [MaxLength(200)]
    public string? HttpMethod { get; set; }
    [MaxLength(500)]
    public string? RequestPath { get; set; }

    // کاربر
    public Guid? UserId { get; set; }
    [MaxLength(200)]
    public string? UserName { get; set; }
    [MaxLength(100)]
    public string? IpAddress { get; set; }

    // متادیتا
    [MaxLength(200)]
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual TenantAppLogSetting? TenantSetting { get; set; }

    [MaxLength(5000)]
    public Dictionary<string, object>? ExtraData { get; set; }

    

}
