using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Commands;

public record UpdateTenantSettingsCommand(
    Guid TenantId,
    string? TimeZone,
    string? Currency,
    string? Language,
    int? MaxUsers,
    int? MaxStorageMb,
    bool? AllowCustomBranding,
    Dictionary<string, string>? CustomSettings
) : IRequest<bool>;
public class UpdateTenantSettingsCommandHandler : IRequestHandler<UpdateTenantSettingsCommand, bool>
{
    private readonly ITenantService _tenantService;

    public UpdateTenantSettingsCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<bool> Handle(UpdateTenantSettingsCommand request, CancellationToken cancellation)
    {
        var result = await _tenantService.UpdateTenant(request, cancellation);
        return result;

    }
}