using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants;



namespace SamaniCrm.Application.Common.Interfaces;
public interface IHostIdentityService
{
    Task<SimpleTenantData?> GetTenantById(Guid tenantId, CancellationToken cancellationToken);
    Task<SimpleTenantData?> GetTenantByTenancyName(string? tenancyName, CancellationToken cancellationToken);
    Task<ExternalProviderDto?> GetExternalProviderAsync(string providerName, CancellationToken cancellationToken);
}
