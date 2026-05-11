namespace SamaniCrm.Application.Features.Tenants.Dtos;

public record TenantSettingsDto(
    Guid TenantId,
    string TimeZone,
    string Currency,
    string Language,
    int MaxUsers,
    int MaxStorageMb,
    bool AllowCustomBranding,
    Dictionary<string, string> CustomSettings
);
