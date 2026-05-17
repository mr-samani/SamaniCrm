using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;


public interface ITenantProvisioningService
{
    Task InitializeTenantProvisionSteps(Guid tenantId, CancellationToken cancellation);
    Task<List<TenantProvisionStepsEnum>> GetPendingProvisionSteps(Guid tenantId, CancellationToken cancellation);
    Task<List<ProvisioningStatusDto>> GetTenantProvisionSteps(Guid tenantId, CancellationToken cancellation,bool? ChackInit);
    Task ProvisionCreateAdminUser(TenantJobProvisioningData request, Guid tenantId, CancellationToken cancellation);
    Task ProvisionIsolatedDatabaseAsync(Tenant tenant, CancellationToken cancellation);
    Task RunMigrationsAsync(Tenant tenant, CancellationToken cancellation);
    Task SeedInitialDataAsync(Tenant tenant, CancellationToken cancellation);
    Task FinalizeAsync(Tenant tenant, CancellationToken cancellation);

    Task<bool> TestDatabaseConnectionAsync(string connectionString, CancellationToken cancellation);
}
