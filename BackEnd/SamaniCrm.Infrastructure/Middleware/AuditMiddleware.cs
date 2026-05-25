using Humanizer;
using Microsoft.AspNetCore.Http;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IAuditLogService _auditLog;

    public AuditMiddleware(
        RequestDelegate next,
        ICurrentUserService currentUser,
        ICurrentTenant currentTenant,
        IAuditLogService auditLog)
    {
        _next = next;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _auditLog = auditLog;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Only log for authenticated requests or significant endpoints
            if (_currentUser.IsAuthenticated || IsAuditableEndpoint(requestPath))
            {
                await _auditLog.LogAsync(new AuditLogEntry
                {
                    CorrelationId = Guid.Parse(correlationId),
                    TenantId = _currentTenant.TenantId,
                    UserId = _currentUser.UserId,
                    Action = $"{requestMethod} {requestPath}",
                    EntityType = ExtractEntityType(requestPath),
                    IpAddress = GetClientIp(context),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    StatusCode = context.Response.StatusCode,
                    DurationMs = (int)stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }

    private bool IsAuditableEndpoint(PathString path)
    {
        var auditablePaths = new[] { "/api/tenants", "/api/users", "/api/roles", "/api/settings" };
        return auditablePaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }

    private string ExtractEntityType(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments?.Length >= 2 ? segments[1].Singularize() : "Unknown";
    }

    private string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        return forwardedFor?.Split(',')[0].Trim()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}
