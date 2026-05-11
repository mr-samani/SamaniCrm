using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.Features.Tenants.Dtos;

public class TenantListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public TenantStatus Status { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
