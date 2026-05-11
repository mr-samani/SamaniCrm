using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Interfaces;

public interface ICurrentTenant
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
    string? TenantName { get; }
    bool HasTenant { get; }

    void SetTenant(Guid tenantId, string slug, string name);
    void Clear();
}