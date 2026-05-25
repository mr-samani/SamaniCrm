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

public record GetTenantDatabaseQuery(Guid TenantId) : IRequest<TenantDatabaseDto>;
public class GetTenantDatabaseQueryHandler : IRequestHandler<GetTenantDatabaseQuery, TenantDatabaseDto>
{
    private readonly ITenantRepository _repository;

    public GetTenantDatabaseQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<TenantDatabaseDto> Handle(GetTenantDatabaseQuery request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _repository.GetByIdAsync(request.TenantId,cancellation)
        //    ?? throw new NotFoundException("Tenant not found");
        //return new TenantDatabaseDto(
        //    tenant.Id, tenant. DatabaseInfo?.Server, tenant.DatabaseInfo?.DatabaseName,
        //    tenant.DatabaseInfo?.LastConnectionTest, tenant.DatabaseInfo?.IsConnectionHealthy);
    }
}

