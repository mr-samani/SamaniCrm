using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.DbContexts;
using System.Data;
using System.Linq.Dynamic.Core;


namespace SamaniCrm.Infrastructure.Services;

public sealed class HostIdentityService : IHostIdentityService
{
    private readonly MasterDbContext _masterDbContext;
    private readonly ITenantDatabaseService _tenantDatabaseService;

    public HostIdentityService(MasterDbContext masterDbContext, ITenantDatabaseService tenantDatabaseService)
    {
        _masterDbContext = masterDbContext;
        _tenantDatabaseService = tenantDatabaseService;
    }

    public async Task<SimpleTenantData?> GetTenantByTenancyName(string? tenancyName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenancyName))
            return null;

        SimpleTenantData? tenant = await _masterDbContext.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.Slug == tenancyName && x.Status == TenantStatus.Active)
            .Select(x => new SimpleTenantData
            {
                Id = x.Id,
                TenancyName = x.Slug,
                TenantName = x.Name,
                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (tenant != null)
        {
            tenant.ConnectionString = _tenantDatabaseService.GetConnectionString(tenant.Id);
        }
        return tenant;
    }

    public async Task<SimpleTenantData?> GetTenantById(Guid tenantId, CancellationToken cancellationToken)
    {
        SimpleTenantData? tenant = await _masterDbContext.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.Id == tenantId && x.Status == TenantStatus.Active)
            .Select(x => new SimpleTenantData
            {
                Id = x.Id,
                TenancyName = x.Slug,
                TenantName = x.Name,
                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (tenant != null)
        {
            tenant.ConnectionString = _tenantDatabaseService.GetConnectionString(tenantId);
        }
        return tenant;
    }

    public async Task<ExternalProviderDto?> GetExternalProviderAsync(string providerName, CancellationToken cancellationToken)
    {
        return await _masterDbContext.ExternalProviders
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.Name.ToLower() == providerName.ToLower() && x.IsActive)
            .Select(x => new ExternalProviderDto
            {
                Name = x.Name,
                TokenEndpoint = x.TokenEndpoint,
                ProviderType = x.ProviderType,
                ClientId = x.ClientId,
                ClientSecret = x.ClientSecret,
                Scopes = x.Scopes,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
