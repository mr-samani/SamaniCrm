namespace SamaniCrm.Application.Features.Tenants.Dtos;

public record ConnectionTestResult(
    Guid TenantId,
    bool Success,
    string? Message,
    long LatencyMs
);
