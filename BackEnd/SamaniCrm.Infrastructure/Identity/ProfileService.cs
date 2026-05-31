using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Duende.IdentityServer.Extensions;



namespace SamaniCrm.Infrastructure.Identity;

public sealed class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    public ProfileService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(
        ProfileDataRequestContext context)
    {
        var userId =
            context.Subject.GetSubjectId();

        var user =
            await _userManager
                .FindByIdAsync(userId);

        if (user == null)
            return;

        var claims = new List<Claim>
        {
            new("lang", user.Lang),
            new("sub", user.Id.ToString()),
            new Claim("preferred_username", user.UserName ?? ""),
            new Claim("name", user.FullName ?? ""),
            new Claim("email", user.Email ?? "")
        };




        if (user.TenantId != null)
        {
            claims.Add(
                new Claim(
                    "tenant_id",
                    user.TenantId.ToString()!));
        }

        var roles =
            await _userManager
                .GetRolesAsync(user);

        claims.AddRange(
            roles.Select(
                r => new Claim("role", r)));

        context.IssuedClaims
            .AddRange(claims);
    }


    public async Task IsActiveAsync(
        IsActiveContext context)
    {
        var user =
            await _userManager
                .FindByIdAsync(
                    context.Subject.GetSubjectId());

        context.IsActive =
            user != null;
    }


}