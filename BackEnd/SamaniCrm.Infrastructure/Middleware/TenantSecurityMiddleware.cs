using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Middleware;

public class TenantSecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantSecurityMiddleware> _logger;

    public TenantSecurityMiddleware(RequestDelegate next, ILogger<TenantSecurityMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenant currentTenant,
        IAuthorizationService authorizationService)
    {
        if (currentTenant.TenantId == null)
        {
            await _next(context);
            return;
        }

        // TODOOOOOOOOOOOOO

        //var tenant = currentTenant.Tenant;

        //// Check IP restriction
        //if (tenant.AllowedIpAddresses?.Any() == true)
        //{
        //    var clientIp = GetClientIpAddress(context);
        //    if (!tenant.AllowedIpAddresses.Contains(clientIp))
        //    {
        //        _logger.LogWarning(
        //            "Access denied for IP {Ip} to tenant {TenantId}",
        //            clientIp, tenant.Id);

        //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //        await context.Response.WriteAsJsonAsync(new { error = "IP address not allowed" });
        //        return;
        //    }
        //}

        //// Check 2FA requirement
        //if (tenant.Require2FA && context.User.Identity?.IsAuthenticated == true)
        //{
        //    var has2FaClaim = context.User.HasClaim("amr", "mfa");
        //    if (!has2FaClaim)
        //    {
        //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //        await context.Response.WriteAsJsonAsync(new
        //        {
        //            error = "2FA is required for this tenant",
        //            require2FA = true
        //        });
        //        return;
        //    }
        //}

        //// Check session timeout
        //var lastActivity = context.User.FindFirst("last_activity")?.Value;
        //if (!string.IsNullOrEmpty(lastActivity) &&
        //    DateTime.TryParse(lastActivity, out var lastActivityTime))
        //{
        //    var timeout = TimeSpan.FromMinutes(tenant.SessionTimeoutMinutes);
        //    if (DateTime.UtcNow - lastActivityTime > timeout)
        //    {
        //        _logger.LogWarning(
        //            "Session timeout for user in tenant {TenantId}",
        //            tenant.Id);

        //        await context.SignOutAsync();
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        await context.Response.WriteAsJsonAsync(new { error = "Session expired" });
        //        return;
        //    }
        //}

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP (behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
