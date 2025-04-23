using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Infrastructure.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");

            var userIdStr = _httpContextAccessor.HttpContext!.User.FindFirst("sub")?.Value
                            ?? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId))
                throw new InvalidOperationException("User ID is not a valid GUID");

            return userId;
        }
    }

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public IEnumerable<Claim> Claims =>
        _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();

    public bool HasPermission(string permission)
    {
        return Claims.Any(c => c.Type == "permission" && c.Value == permission);
    }
}
