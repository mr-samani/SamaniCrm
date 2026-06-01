namespace SamaniCrm.Core.Shared.Enums;

/// <summary>
/// نمای کلی از نوع رویدادهای امنیتی
/// </summary>
public enum SecurityEventType
{
    LoginSuccess,
    LoginFailed,
    Logout,
    PasswordChange,
    AccountLocked,
    AccessDenied,
    DataExport,
    PrivilegeEscalation,
    ApiKeyGenerated,
    ApiKeyRevoked
}
