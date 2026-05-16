namespace SamaniCrm.Core.Shared.DTOs;

public class TenantJobProvisioningData
{
    public Guid TenantId { get; set; }
    public required string Slug { get; set; }
    public required string  AdminEmail { get; set; }
    public required string  AdminFirstName { get; set; }
    public required string  AdminLastName { get; set; }
    public required string  AdminPassword { get; set; }
    public required string  AdminUserName { get; set; }
    public required string  AdminMobile { get; set; }
    public required string  Address { get; set; }
}
