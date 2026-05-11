using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Commands;

public record SuspendTenantCommand(Guid Id, string? Reason = null) : IRequest<bool>;
public class SuspendTenantCommandHandler : IRequestHandler<SuspendTenantCommand, bool>
{
    private readonly ITenantService _tenantService;

    public SuspendTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<bool> Handle(SuspendTenantCommand request, CancellationToken cancellation)
    {
        var result = await _tenantService.ActiveOrDeactiveTenant(request.Id, false, request.Reason, cancellation);
        return result;
    }
}