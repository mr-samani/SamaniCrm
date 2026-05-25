using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Queries;

public record GetTenantUsageQuery(Guid TenantId) : IRequest<TenantUsageDto>;

public class GetTenantUsageQueryHandler : IRequestHandler<GetTenantUsageQuery, TenantUsageDto>
{
    private readonly ITenantRepository _repository;

    public GetTenantUsageQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<TenantUsageDto> Handle(GetTenantUsageQuery request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _repository.GetByIdAsync(request.TenantId)
        //    ?? throw new NotFoundException("Tenant not found");
        //return new TenantUsageDto(
        //    tenant.Id, tenant.Usage.UserCount, tenant.Settings.MaxUsers,
        //    tenant.Usage.StorageUsedMb, tenant.Settings.MaxStorageMb,
        //    tenant.Usage.ApiCallsThisMonth, tenant.Usage.LastActivityAt);
    }
}
