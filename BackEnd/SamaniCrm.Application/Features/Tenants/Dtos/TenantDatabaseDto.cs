namespace SamaniCrm.Application.Features.Tenants.Dtos;

public record TenantDatabaseDto(
    Guid TenantId,
    string? Server,
    string? DatabaseName,
    DateTime? LastConnectionTest,
    bool? IsConnectionHealthy
);
