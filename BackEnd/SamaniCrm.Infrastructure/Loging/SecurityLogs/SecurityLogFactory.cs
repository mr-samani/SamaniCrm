using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;


namespace SamaniCrm.Infrastructure.Loging.SecurityLogs;


public class SecurityLogFactory : ISecurityLogFactory
{
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SecurityLogFactory(ICurrentUserService currentUser,
        ICurrentTenant currentTenant,
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _httpContextAccessor = httpContextAccessor;
    }

    public SecurityLogDto Create(SecurityEventType eventType, LogLevel sererity,
        bool IsSuccessful, System.Net.HttpStatusCode statusCode,
        string? error = null)
    {
        var context = _httpContextAccessor.HttpContext;

        var userAgent = context?.Request.Headers.UserAgent.ToString();
        var path = context?.Request.Path.ToString();
        var action = $"{context?.Request.Method} {context?.Request.Path}";
        var ip = context?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var resource = "";
        var correlationId = context?.TraceIdentifier ?? "Unknown";


        return new SecurityLogDto()
        {
            Id = Guid.NewGuid(),
            TenantId = _currentTenant.TenantId,
            UserId = _currentUser.UserId,
            Username = _currentUser.UserName,
            CorrelationId = correlationId,
            UserAgent = userAgent,
            IpAddress = ip,
            Action = action,
            CreatedAt = DateTime.UtcNow,
            Message = error,
            EventType = eventType,
            IsSuccessful = IsSuccessful,
            Resource = resource,
            Severity = sererity,
            StatusCode = (int)statusCode,
        };
    }
}
