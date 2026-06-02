using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Consts;

namespace SamaniCrm.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("sub")
                ?.Value;

            return Guid.TryParse(sub, out var id)
                ? id
                : null;
        }
    }

    public string? UserName =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst("preferred_username")
            ?.Value;

    public string Lang =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst("lang")
            ?.Value
        ?? "fa-IR";


    public Guid? TenantId
    {
        get
        {
            var tenantId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("tenant_id")
                ?.Value;

            return Guid.TryParse(tenantId, out var id)
                ? id
                : null;
        }
    }


    public bool IsDelegated =>
    _httpContextAccessor.HttpContext?
        .User
        .HasClaim("is_delegated", "true")
    ?? false;

    public Guid? DelegatorId
    {
        get
        {
            var value =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue("delegator_id");

            return string.IsNullOrEmpty(value)
                ? null
                : Guid.Parse(value);
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .IsAuthenticated
        ?? false;
    public bool IsHost => TenantId == null;

    public bool IsTenantAdmin
    {
        get
        {
            string? roles =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue("roles");

            if (string.IsNullOrEmpty(roles))
            {
                return false;
            }

            return roles.Contains(AppRoles.TenantAdministrator);
        }
    }
}