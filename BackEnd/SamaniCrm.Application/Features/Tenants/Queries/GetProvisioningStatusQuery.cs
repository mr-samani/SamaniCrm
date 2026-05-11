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

public record GetProvisioningStatusQuery(Guid TenantId) : IRequest<ProvisioningStatusDto>;
public class GetProvisioningStatusQueryHandler : IRequestHandler<GetProvisioningStatusQuery, ProvisioningStatusDto>
{
    private readonly ITenantRepository _repository;

    public GetProvisioningStatusQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<ProvisioningStatusDto> Handle(GetProvisioningStatusQuery request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _repository.GetByIdAsync(request.TenantId)
        //    ?? throw new NotFoundException("Tenant not found");
        //return new ProvisioningStatusDto(
        //    tenant.Id, tenant.Provisioning.Status.ToString(),
        //    tenant.Provisioning.StartedAt, tenant.Provisioning.CompletedAt,
        //    tenant.Provisioning.ErrorMessage, tenant.Provisioning.RetryCount);
    }
}
