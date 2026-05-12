using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services.TenantService;


public record TenantRsolverDto(Guid Id, string Name, string Slug, string Email, TenantStatus Status);


public interface ITenantResolver
{
    Guid? TenantId { get; set; }
    Task<TenantRsolverDto?> ResolveAsync(HttpContext context);
    Task<TenantRsolverDto?> ResolveByIdAsync(Guid tenantId);
    Task<TenantRsolverDto?> ResolveBySlugAsync(string slug);
}


public class TenantResolver : ITenantResolver
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Guid? TenantId { get; set; }
    public TenantResolver(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<TenantRsolverDto?> ResolveAsync(HttpContext context)
    {
        TenantRsolverDto? tenant = null;
        // 1️⃣ اول از Header بخون (X-Tenant-Slug)
        var tenantSlug = context.Request.Headers["X-Tenant-Slug"].FirstOrDefault();
        if (!string.IsNullOrEmpty(tenantSlug))
        {
            tenant = await ResolveBySlugAsync(tenantSlug);
        }

        // 2️⃣ از Subdomain بخون (tenant.example.com)
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);
        if (!string.IsNullOrEmpty(subdomain))
        {
            tenant = await ResolveBySlugAsync(subdomain);
        }

        // 3️⃣ از Route بخون (/api/tenant-slug/...)
        var routeSlug = context.Request.RouteValues["tenant"]?.ToString();
        if (!string.IsNullOrEmpty(routeSlug))
        {
            tenant = await ResolveBySlugAsync(routeSlug);
        }

        TenantId = tenant != null ? tenant.Id : null;

        return tenant;
    }



    private static string? ExtractSubdomain(string host)
    {
        // مثال: tenant.example.com → tenant
        var parts = host.Split('.');
        if (parts.Length > 2)
        {
            return parts[0];
        }
        return null;
    }


    public async Task<TenantRsolverDto?> ResolveByIdAsync(Guid tenantId)
    {
        var tenant = await _context.Tenants
            .Select(s => new TenantRsolverDto(s.Id, s.Name, s.Slug, s.Email, s.Status))
             .AsNoTracking()
             .FirstOrDefaultAsync(t => t.Id == tenantId);


        return tenant;
    }

    public async Task<TenantRsolverDto?> ResolveBySlugAsync(string slug)
    {
        var tenant = await _context.Tenants
            .Select(s => new TenantRsolverDto(s.Id, s.Name, s.Slug, s.Email, s.Status))
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug.ToLower() == slug.ToLower());

        return tenant;
    }


}