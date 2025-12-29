using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using MimeDetective.Diagnostics;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using static Duende.IdentityModel.OidcConstants;

namespace SamaniCrm.Infrastructure.Persistence;

public static class SeedExternalProviders
{
    public static async Task TrySeedAsync(ApplicationDbContext dbContext)
    {
        Console.WriteLine("Seeding external providers...");

        var providers = GetProviders();


        var existingKeys = await dbContext.ExternalProviders.Select(x => x.Name).ToListAsync();

        // فقط مواردی که وجود ندارند را اضافه می‌کنیم
        var toAdd = providers
            .Where(x => !existingKeys.Contains(x.Name))
            .ToList();

        if (toAdd.Any())
        {
            await dbContext.ExternalProviders.AddRangeAsync(toAdd);
            await dbContext.SaveChangesAsync();
        }


        Console.WriteLine("seed external providers ended");
    }



    public static List<ExternalProvider> GetProviders()
    {
        return new List<ExternalProvider>
        {
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "Google",
                DisplayName = "Google",
                AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth",
                TokenEndpoint = "https://oauth2.googleapis.com/token",
                UserInfoEndpoint = "https://openidconnect.googleapis.com/v1/userinfo",
                Scopes = "openid profile email",
                ProviderType = ExternalProviderTypeEnum.Google
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "Microsoft",
                DisplayName = "Microsoft",
                AuthorizationEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize",
                TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token",
                UserInfoEndpoint = "https://graph.microsoft.com/oidc/userinfo",
                Scopes = "openid profile email",
                ProviderType = ExternalProviderTypeEnum.Microsoft
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "Facebook",
                DisplayName = "Facebook",
                AuthorizationEndpoint = "https://www.facebook.com/v15.0/dialog/oauth",
                TokenEndpoint = "https://graph.facebook.com/v15.0/oauth/access_token",
                UserInfoEndpoint = "https://graph.facebook.com/me?fields=id,name,email,picture",
                Scopes = "email public_profile",
                ProviderType = ExternalProviderTypeEnum.Facebook
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "GitHub",
                DisplayName = "GitHub",
                AuthorizationEndpoint = "https://github.com/login/oauth/authorize",
                TokenEndpoint = "https://github.com/login/oauth/access_token",
                UserInfoEndpoint = "https://api.github.com/user",
                Scopes = "read:user user:email",
                ProviderType = ExternalProviderTypeEnum.GitHub
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "LinkedIn",
                DisplayName = "LinkedIn",
                AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization",
                TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken",
                UserInfoEndpoint = "https://api.linkedin.com/v2/me",
                Scopes = "email profile",// "r_liteprofile r_emailaddress",
                ProviderType = ExternalProviderTypeEnum.LinkedIn
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "Twitter",
                DisplayName = "Twitter (X)",
                AuthorizationEndpoint = "https://twitter.com/i/oauth2/authorize",
                TokenEndpoint = "https://api.twitter.com/2/oauth2/token",
                UserInfoEndpoint = "https://api.twitter.com/2/users/me",
                Scopes = "tweet.read users.read offline.access",
                ProviderType = ExternalProviderTypeEnum.Twitter
            },
            new ExternalProvider
            {
                Id = Guid.NewGuid(),
                Name = "SamaniAuth",
                DisplayName = "KeyClock",
                AuthorizationEndpoint = "https://localhost:8443/realms/master/protocol/openid-connect/auth",
                TokenEndpoint = "https://localhost:8443/realms/master/protocol/openid-connect/token",
                UserInfoEndpoint = "https://localhost:8443/realms/master/protocol/openid-connect/userinfo",
                Scopes = "openid profile email",
                ProviderType = ExternalProviderTypeEnum.OpenIdConnect,
                IsActive = false,
                ClientId = "SamaniAuth",
                ClientSecret = "Zf90JkD87avWLigGEQe3Tk6TmqkQJInu",
                ResponseMode = "query",
                ResponseType = "code",
                UsePkce = true
            }

        };
    }


}
