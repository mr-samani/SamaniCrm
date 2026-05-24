using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Connections;
 
 

public interface IConnectionManager
{
    // ─────────────────────────────────────────────────────────────
    // Connection Tracking
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// ثبت یک کانکشن جدید
    /// </summary>
    Task<bool> AddConnectionAsync(string connectionId, IConnectionInfo connectionInfo);

    /// <summary>
    /// حذف یک کانکشن
    /// </summary>
    Task<bool> RemoveConnectionAsync(string connectionId);

    /// <summary>
    /// آپدیت آخرین فعالیت کانکشن
    /// </summary>
    Task UpdateLastActivityAsync(string connectionId);

    /// <summary>
    /// دریافت اطلاعات یک کانکشن
    /// </summary>
    IConnectionInfo? GetConnection(string connectionId);

    /// <summary>
    /// دریافت تمام کانکشن‌های یک کاربر
    /// </summary>
    IReadOnlyList<IConnectionInfo> GetUserConnections(string userId);

    /// <summary>
    /// تعداد کانکشن‌های فعال یک کاربر
    /// </summary>
    int GetUserConnectionCount(string userId);

    /// <summary>
    /// آیا کاربر آنلاین است؟
    /// </summary>
    bool IsUserOnline(string userId);

    /// <summary>
    /// دریافت تمام کانکشن‌های فعال
    /// </summary>
    IReadOnlyList<IConnectionInfo> GetAllConnections();

    // ─────────────────────────────────────────────────────────────
    // Device & Session Management
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// ثبت سشن جدید برای دستگاه
    /// </summary>
    Task<IDeviceSession> CreateSessionAsync(string userId, string deviceId,
        string deviceType, string deviceName, string ipAddress);

    /// <summary>
    /// دریافت تمام سشن‌های فعال یک کاربر
    /// </summary>
    IReadOnlyList<IDeviceSession> GetUserSessions(string userId);

    /// <summary>
    /// دریافت خلاصه سشن‌های کاربر
    /// </summary>
    UserSessionSummary GetUserSessionSummary(string userId);

    /// <summary>
    /// بستن یک سشن خاص (تمام کانکشن‌های آن)
    /// </summary>
    Task<bool> TerminateSessionAsync(string userId, string sessionId);

    /// <summary>
    /// بستن تمام سشن‌های یک کاربر به جز سشن فعلی
    /// </summary>
    Task<int> TerminateOtherSessionsAsync(string userId, string currentSessionId);

    /// <summary>
    /// بستن تمام سشن‌های یک کاربر
    /// </summary>
    Task<int> TerminateAllUserSessionsAsync(string userId);

    // ─────────────────────────────────────────────────────────────
    // Group Management
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// افزودن کانکشن به گروه
    /// </summary>
    Task AddToGroupAsync(string connectionId, string groupName);

    /// <summary>
    /// حذف کانکشن از گروه
    /// </summary>
    Task RemoveFromGroupAsync(string connectionId, string groupName);

    /// <summary>
    /// دریافت اعضای گروه
    /// </summary>
    IReadOnlyList<string> GetGroupMembers(string groupName);

    /// <summary>
    /// دریافت تمام گروه‌ها
    /// </summary>
    IReadOnlyList<string> GetAllGroups();

    // ─────────────────────────────────────────────────────────────
    // Statistics
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// آمار کلی
    /// </summary>
    ConnectionStatistics GetStatistics();
}

public class ConnectionStatistics
{
    public int TotalConnections { get; set; }
    public int TotalUsers { get; set; }
    public int TotalSessions { get; set; }
    public int TotalGroups { get; set; }
    public Dictionary<string, int> ConnectionsPerUser { get; set; } = new();
    public Dictionary<string, int> ConnectionsByDeviceType { get; set; } = new();
}