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

public record GetTenantSettingsQuery(Guid TenantId) : IRequest<TenantSettingsDto>;
public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, TenantSettingsDto>
{
    private readonly ITenantRepository _repository;

    public GetTenantSettingsQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<TenantSettingsDto> Handle(GetTenantSettingsQuery request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _repository.GetByIdAsync(request.TenantId)
        //    ?? throw new NotFoundException("Tenant not found");
        //return new TenantSettingsDto(
        //    tenant.Id, tenant.Settings.TimeZone, tenant.Settings.Currency,
        //    tenant.Settings.Language, tenant.Settings.MaxUsers, tenant.Settings.MaxStorageMb,
        //    tenant.Settings.AllowCustomBranding, tenant.Settings.CustomSettings);
    }
}

