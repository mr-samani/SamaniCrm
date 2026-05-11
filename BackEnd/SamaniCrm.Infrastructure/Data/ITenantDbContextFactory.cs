namespace SamaniCrm.Infrastructure.Data;

public interface ITenantDbContextFactory
{
    ApplicationDbContext CreateDbContext(Guid tenantId);
    ApplicationDbContext CreateDbContext(string connectionString);
    Task<ApplicationDbContext> CreateDbContextAsync(Guid tenantId, CancellationToken cancellation);
    Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellation);
}
