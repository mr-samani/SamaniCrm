using SamaniCrm.Application.Features.Tenants.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services.TenantService;


public class CurrentTenant : ICurrentTenant
{
    private Guid? _tenantId;
    private string? _tenantSlug;
    private string? _tenantName;

    public Guid? TenantId => _tenantId;
    public string? TenantSlug => _tenantSlug;
    public string? TenantName => _tenantName;
    public bool HasTenant => _tenantId.HasValue;

    public void SetTenant(Guid tenantId, string slug, string name)
    {
        _tenantId = tenantId;
        _tenantSlug = slug;
        _tenantName = name;
    }

    public void Clear()
    {
        _tenantId = null;
        _tenantSlug = null;
        _tenantName = null;
    }

    public Guid? GetCurrentTenantId()
    {
        return _tenantId;
    }
}
