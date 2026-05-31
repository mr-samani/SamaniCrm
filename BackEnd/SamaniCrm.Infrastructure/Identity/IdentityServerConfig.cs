using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Identity;

using Duende.IdentityServer.Models;

public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),

        new IdentityResource(
            "roles",
            ["role"]),

        new IdentityResource(
            "tenant",
            ["tenant_id"]),

        new IdentityResource(
            "lang",
            ["lang"])
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("api")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("api")
        {
            Scopes = { "api" },

            UserClaims =
            {
                "role",
                "tenant_id",
                "lang"
            }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "angular",

            ClientName = "Angular SPA",

            AllowedGrantTypes = GrantTypes.Code,

            RequirePkce = true,

            RequireClientSecret = false,

            RedirectUris =
            {
                "https://localhost:5753/auth-callback"
            },

            PostLogoutRedirectUris =
            {
                "https://localhost:5753"
            },

            AllowedCorsOrigins =
            {
                "https://localhost:5753"
            },

            AllowedScopes =
            {
                "openid",
                "profile",
                "roles",
                "tenant",
                "lang",
                "api",
                "offline_access"
            },

            AllowOfflineAccess = true,

            RefreshTokenUsage =
                TokenUsage.OneTimeOnly,

            RefreshTokenExpiration =
                TokenExpiration.Sliding,

            SlidingRefreshTokenLifetime =
                2592000,

            AccessTokenLifetime = 3600
        }
    ];
}