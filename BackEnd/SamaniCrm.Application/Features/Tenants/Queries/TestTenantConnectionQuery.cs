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

public record TestTenantConnectionQuery(Guid TenantId) : IRequest<ConnectionTestResult>;
public class TestTenantConnectionQueryHandler : IRequestHandler<TestTenantConnectionQuery, ConnectionTestResult>
{
    private readonly ITenantRepository _repository;
    //private readonly IDatabaseConnectionTester _connectionTester;

    public TestTenantConnectionQueryHandler(ITenantRepository repository
        //IDatabaseConnectionTester connectionTester
        )
    {
        _repository = repository;
       // _connectionTester = connectionTester;
    }

    public async Task<ConnectionTestResult> Handle(TestTenantConnectionQuery request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
        //var tenant = await _repository.GetByIdAsync(request.TenantId)
        //    ?? throw new NotFoundException("Tenant not found");

        //var (success, message, latencyMs) = await _connectionTester.TestConnectionAsync(
        //    tenant.DatabaseInfo?.ConnectionString ?? "");

        //tenant.DatabaseInfo ??= new TenantDatabaseInfo();
        //tenant.DatabaseInfo.LastConnectionTest = DateTime.UtcNow;
        //tenant.DatabaseInfo.IsConnectionHealthy = success;

        //return new ConnectionTestResult(tenant.Id, success, message, latencyMs);
    }
}


