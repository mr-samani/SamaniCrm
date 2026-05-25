using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Queries;

public record GetProvisioningStatusQuery(Guid TenantId) : IRequest<List<ProvisioningStatusDto>>;
public class GetProvisioningStatusQueryHandler : IRequestHandler<GetProvisioningStatusQuery, List<ProvisioningStatusDto>>
{
    private readonly ITenantProvisioningService _tenantProvisioningService;

    public GetProvisioningStatusQueryHandler(
        ITenantProvisioningService tenantProvisioningService)
    {
        _tenantProvisioningService = tenantProvisioningService;
    }

    public async Task<List<ProvisioningStatusDto>> Handle(GetProvisioningStatusQuery request, CancellationToken cancellation)
    {

        var result = await _tenantProvisioningService.GetTenantProvisionSteps(request.TenantId, cancellation, true);

        return result;
    }
}
