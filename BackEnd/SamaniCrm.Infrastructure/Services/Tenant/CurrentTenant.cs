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
    private string? _connectionString;

    public Guid? TenantId => _tenantId;
    public string? TenantSlug => _tenantSlug;
    public string? TenantName => _tenantName;
    public string? ConnectionString => _connectionString;

    public bool HasTenant => _tenantId.HasValue;

    public void SetTenant(Guid tenantId, string slug, string name,string connectionString)
    {
        _tenantId = tenantId;
        _tenantSlug = slug;
        _tenantName = name;
        _connectionString = connectionString;
    }

    public void Clear()
    {
        _tenantId = null;
        _tenantSlug = null;
        _tenantName = null;
    }
}
