namespace SamaniCrm.Application.NotificationManager.Interfaces;

public interface INotificationSender
{
    // ارسال به یک کاربر (تمام دستگاه‌ها)
    Task SendToUserAsync(string userId, string method, object? message);

    // ارسال به یک کاربر در دستگاه خاص
    Task SendToUserDeviceAsync(string userId, string deviceId, string method, object? message);

    // ارسال به چند کاربر
    Task SendToUsersAsync(IEnumerable<string> userIds, string method, object? message);

    // ارسال به گروه
    Task SendToGroupAsync(string groupName, string method, object? message);

    // ارسال به همه
    Task SendToAllAsync(string method, object? message);

    // ارسال به کاربران آنلاین
    Task SendToOnlineUsersAsync(string method, object? message);
}
