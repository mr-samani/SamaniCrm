namespace SamaniCrm.Application.Features.Tenants.Dtos;

public record ProvisioningStatusDto(
    Guid TenantId,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? ErrorMessage,
    int RetryCount
);
