using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Queries;

namespace SamaniCrm.Core.Shared.Interfaces.Tenant;

public interface ITenantRepository
{
    Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellation);
    Task<TenantDto?> GetBySlugAsync(string slug, CancellationToken cancellation);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellation, Guid? excludeId = null);

    Task<PaginatedResult<TenantListDto>> GetAllAsync(TenantListQuery filter, CancellationToken cancellation);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellation);
}
