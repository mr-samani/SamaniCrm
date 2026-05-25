using SamaniCrm.Application.Features.Tenants.Commands;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ITenantService
{
    Task<CreateTenantResponse> CreateTenantAsync(CreateTenantCommand request, CancellationToken cancellation);

    Task<bool> ActiveOrDeactiveTenant(Guid id,bool isSusspend, string? reason, CancellationToken cancellation);
    Task<bool> RetryProvisioning(Guid id, CancellationToken cancellation);
    Task<bool> UpdateTenant(UpdateTenantSettingsCommand request, CancellationToken cancellation);
}
