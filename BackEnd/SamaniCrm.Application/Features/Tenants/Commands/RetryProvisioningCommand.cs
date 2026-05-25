using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Commands;

public record RetryProvisioningCommand(Guid Id) : IRequest<bool>;
public class RetryProvisioningCommandHandler : IRequestHandler<RetryProvisioningCommand, bool>
{
    private readonly ITenantService _tenantService;

    public RetryProvisioningCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<bool> Handle(RetryProvisioningCommand request, CancellationToken cancellation)
    {

        var result = await _tenantService.RetryProvisioning(request.Id, cancellation);
        return result;

    }
}

