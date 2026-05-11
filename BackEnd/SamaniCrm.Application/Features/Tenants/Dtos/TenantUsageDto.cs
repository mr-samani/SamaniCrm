namespace SamaniCrm.Application.Features.Tenants.Dtos;

public record TenantUsageDto(
    Guid TenantId,
    int UserCount,
    int MaxUsers,
    long StorageUsedMb,
    int MaxStorageMb,
    int ApiCallsThisMonth,
    DateTime? LastActivityAt
);
