using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class SecurityLogDto
{
    public Guid Id { get; set; }
    /// <summary>
    /// شناسه همبستگی برای ردیابی در میکروسرویس‌ها
    /// </summary>
    public required string CorrelationId { get; set; }

    public Guid? TenantId { get; set; }
    /// <summary>
    /// نوع رویداد امنیتی
    /// </summary>
    public required SecurityEventType EventType { get; set; }

    /// <summary>
    /// سطح اهمیت
    /// </summary>
    public required LogLevel Severity { get; set; }

    /// <summary>
    /// شناسه کاربر فعلی (اگر لاگ مربوط به کاربر است)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// نام کاربری (برای نمایش و جستجو)
    /// </summary>
    [StringLength(256)]
    public string? Username { get; set; }

    /// <summary>
    /// آدرس IP کلاینت
    /// </summary>
    [StringLength(45)] // پشتیبانی از IPv6
    public required string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent مرورگر/کلاینت
    /// </summary>
    [StringLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// عملیات انجام شده (مثلاً: GET /api/users)
    /// </summary>
    [StringLength(256)]
    public required string Action { get; set; } = string.Empty;

    /// <summary>
    /// منبع مورد دسترسی
    /// </summary>
    [StringLength(256)]
    public string? Resource { get; set; }

    /// <summary>
    /// کد وضعیت HTTP یا نتیجه عملیات
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// آیا عملیات موفق بود؟
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// پیام خطا (در صورت وجود)
    /// </summary>
    [StringLength(2048)]
    public string? ErrorMessage { get; set; }



    /// <summary>
    /// زمان ایجاد لاگ (با تایم‌زون)
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// سیستم یا سرویس ایجادکننده لاگ
    /// </summary>
    [StringLength(100)]
    public string CreatedBy { get; set; } = "System";

}

