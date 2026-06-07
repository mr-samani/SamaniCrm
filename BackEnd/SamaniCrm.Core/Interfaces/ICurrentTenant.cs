
namespace SamaniCrm.Core.Shared.Interfaces;

public interface ICurrentTenant
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
    string? TenantName { get; }
    string? ConnectionString { get; }
    bool HasTenant { get; }

    IDisposable Change(
        Guid? tenantId,
        string? tenantSlug = null,
        string? tenantName = null,
        string? connectionString = null);

    void Clear();

    string GetCurrentConnectionString();
}