using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SamaniCrm.Domain.Entities;

public class TenantLogSetting:IMayHaveTenant
{
    public int Id { get; set; }
    public Guid? TenantId { get; set; }

    // سطوح فعال لاگ‌نویسی (بیت‌ماسک)
    public LogLevelMask EnabledLevels { get; set; } = LogLevelMask.All;

    // مقصدهای فعال لاگ
    public LogSinkMask EnabledSinks { get; set; } = LogSinkMask.All;

    // مدت نگهداری (روز) - null = نامحدود
    public int? RetentionDays { get; set; } = 30;

    // تنظیمات اضافی JSON
    [MaxLength(1000)]
    public string? CustomSettings { get; set; }

    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}