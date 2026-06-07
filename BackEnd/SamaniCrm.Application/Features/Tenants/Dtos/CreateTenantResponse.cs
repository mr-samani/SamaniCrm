using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.Features.Tenants;

public class CreateTenantResponse
{
    public Guid TenantId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public ProvisioningStatus ProvisioningStatus { get; set; }
    public DateTime? TrialEndsAt { get; set; }
}

public class SimpleTenantData
{
    public required Guid Id { get; set; }
    public required string TenancyName { get; set; }
    public required string TenantName { get; set; }
    public required TenantStatus Status { get; set; }
    public string? ConnectionString { get; set; }
}