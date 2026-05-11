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

public record ActivateTenantCommand(Guid Id, string Description) : IRequest<bool>;
public class ActivateTenantCommandHandler : IRequestHandler<ActivateTenantCommand, bool>
{
    private readonly ITenantService _tenantService;

    public ActivateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<bool> Handle(ActivateTenantCommand request, CancellationToken cancellation)
    {
        var result = await _tenantService.ActiveOrDeactiveTenant(request.Id, true, request.Description, cancellation);
        return result;
    }
}