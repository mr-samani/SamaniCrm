using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTools;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Infrastructure.Services.TenantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    public TenantResolverMiddleware(RequestDelegate next, ILogger<TenantResolverMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ICurrentTenant currentTenant)
    {
        // Skip tenant resolution for public endpoints
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null
                        // TODO:
                        //||
                        //endpoint?.Metadata.GetMetadata<PublicEndpointAttribute>() != null
                        )
        {
            await _next(context);
            return;
        }

        // Try to resolve tenant from multiple sources (in order of priority)
        TenantRsolverDto? tenant = null;

        // 1. From subdomain
        tenant = await ResolveFromSubdomainAsync(context, tenantResolver);

        // 2. From route
        if (tenant == null)
        {
            tenant = await ResolveFromRouteAsync(context, tenantResolver);
        }

        // 3. From authenticated user's session (MOST SECURE)
        if (tenant == null && context.User.Identity?.IsAuthenticated == true)
        {
            tenant = await ResolveFromUserSessionAsync(context, tenantResolver);
        }

        // 4. From header (only for specific internal services)
        if (tenant == null && context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (IsInternalServiceRequest(context))
            {
                tenant = await tenantResolver.ResolveByIdAsync(Guid.Parse(tenantIdHeader!));
            }
        }

        //if (tenant == null)
        //{
        //    _logger.LogWarning("Tenant could not be resolved for request {Path}", context.Request.Path);
        //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
        //    await context.Response.WriteAsJsonAsync(new { error = "Tenant not identified" });
        //    return;
        //}

        // Validate tenant status
        if (tenant != null && tenant.Status != TenantStatus.Active)
        {
            _logger.LogWarning("Tenant {TenantId} is not active", tenant.Id);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant is not active" });
            return;
        }

        if (tenant != null)
        {
            // Set current tenant
            currentTenant.SetTenant(tenant.Id, tenant.Slug, tenant.Name);

            // Add tenant info to response headers (for debugging)
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Tenant-Id"] = tenant.Id.ToString();
                context.Response.Headers["X-Tenant-Slug"] = tenant.Slug;
                return Task.CompletedTask;
            });
        }



        await _next(context);
    }

    private async Task<TenantRsolverDto?> ResolveFromSubdomainAsync(
        HttpContext context, ITenantResolver resolver)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        // Format: tenant-slug.domain.com
        if (parts.Length >= 3 && !parts[0].Equals("www", StringComparison.OrdinalIgnoreCase))
        {
            var slug = parts[0];
            return await resolver.ResolveBySlugAsync(slug);
        }

        return null;
    }

    private async Task<TenantRsolverDto?> ResolveFromRouteAsync(
        HttpContext context, ITenantResolver resolver)
    {
        if (context.Request.RouteValues.TryGetValue("tenantSlug", out var slug))
        {
            return await resolver.ResolveBySlugAsync(slug?.ToString()!);
        }

        if (context.Request.RouteValues.TryGetValue("tenantId", out var id))
        {
            return await resolver.ResolveByIdAsync(Guid.Parse(id?.ToString()!));
        }

        return null;
    }

    private async Task<TenantRsolverDto?> ResolveFromUserSessionAsync(
        HttpContext context, ITenantResolver resolver)
    {
        // Get tenant ID from user's claims (set during authentication)
        var tenantIdClaim = context.User.FindFirst("tenant_id");

        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            return await resolver.ResolveByIdAsync(tenantId);
        }

        return null;
    }


    /// <summary>
    /// IP های خصوصی (Private) استاندارد
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <remarks>
    /// 127.0.0.1 / ::1     = لوکال‌هاست (همیشه خود سرور)
    /// 10.0.0.0/8          = شبکه‌های بزرگ خصوصی
    /// 172.16.0.0/12       = شبکه‌های خصوصی متوسط
    /// 192.168.0.0/16      = شبکه‌های کوچک(مثلاً مودم‌ها)
    /// </remarks>
    /// <returns></returns>
    private bool IsInternalServiceRequest(HttpContext context)
    {
        // Only allow tenant header from internal network or specific IPs
        var remoteIp = context.Connection.RemoteIpAddress;
        var internalIps = new[] { "127.0.0.1", "::1", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16" };
        // TODO must be check        
        return internalIps.Any(ip => IPAddressRange.Parse(ip).Contains(remoteIp));
    }
}
