using Microsoft.Extensions.Logging;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SamaniCrm.Domain.Entities;



public class SecurityLogEntry:IMayHaveTenant
{
    public Guid Id { get; set; }
    /// <summary>
    /// شناسه همبستگی برای ردیابی در میکروسرویس‌ها
    /// </summary>
    [Required]
    public required string CorrelationId { get; set; }

    public Guid? TenantId { get; set; }

    /// <summary>
    /// نوع رویداد امنیتی
    /// </summary>
    [Required]
    public SecurityEventType EventType { get; set; }

    /// <summary>
    /// سطح اهمیت
    /// </summary>
    [Required]
    public LogLevel Severity { get; set; }

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
    [Required]
    [StringLength(45)] // پشتیبانی از IPv6
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent مرورگر/کلاینت
    /// </summary>
    [StringLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// عملیات انجام شده (مثلاً: GET /api/users)
    /// </summary>
    [Required]
    [StringLength(256)]
    public string Action { get; set; } = string.Empty;

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
    public string? Message { get; set; }



    /// <summary>
    /// زمان ایجاد لاگ (با تایم‌زون)
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// سیستم یا سرویس ایجادکننده لاگ
    /// </summary>
    [StringLength(100)]
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// هش SHA256 برای اطمینان از عدم تغییر لاگ (Integrity)
    /// </summary>
    [Required]
    [StringLength(64)]
    public string IntegrityHash { get; set; } = string.Empty;

    /// <summary>
    /// محاسبه هش برای اطمینان از تغییرناپذیری
    /// </summary>
    public void CalculateIntegrityHash()
    {
        var dataToHash = $"{Id}{TenantId}{EventType}{UserId}{IpAddress}{Action}{CreatedAt}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataToHash));
        IntegrityHash = Convert.ToHexString(hashBytes);
    }
}
