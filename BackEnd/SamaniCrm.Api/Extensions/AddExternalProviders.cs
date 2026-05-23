using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalProviders(this IServiceCollection services, IConfiguration config)
    {
        // load external provider configs (sync for bootstrap; or async with factory)
        using (var sp = services.BuildServiceProvider())
        {
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var secretStore = sp.GetRequiredService<ISecretStore>(); // wrapper for KeyVault / DPAPI
            var providers = db.ExternalProviders.Where(p => p.IsActive).ToList();

            foreach (var provider in providers)
            {
                switch (provider.ProviderType)
                {
                    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-9.0
                    case ExternalProviderTypeEnum.Google:
                        services.AddAuthentication().AddGoogle(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Google:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Google:ClientSecret");
                            options.CallbackPath = provider.CallbackPath ?? $"/signin-{provider.Scheme}";
                            options.SignInScheme = IdentityConstants.ExternalScheme;
                            // map claims if needed
                        });
                        break;
                    // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-10.0
                    case ExternalProviderTypeEnum.Microsoft:
                        services.AddAuthentication().AddMicrosoftAccount(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Microsoft:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Microsoft:ClientSecret");
                        });
                        break;

                    case ExternalProviderTypeEnum.Facebook:
                        services.AddAuthentication().AddFacebook(options =>
                        {
                            options.ClientId = secretStore.GetSecret("Facebook:ClientId");
                            options.ClientSecret = secretStore.GetSecret("Facebook:ClientSecret");
                        });
                        break;

                    case ExternalProviderTypeEnum.GitHub:
                        services.AddAuthentication().AddGitHub(options =>
                        {
                            options.ClientId = secretStore.GetSecret("GitHub:ClientId");
                            options.ClientSecret = secretStore.GetSecret("GitHub:ClientSecret");
                        });
                        break;
                    //https://learn.microsoft.com/en-us/linkedin/shared/authentication/client-credentials-flow?tabs=HTTPS1
                    case ExternalProviderTypeEnum.LinkedIn:
                        services.AddAuthentication().AddLinkedIn(options =>
                        {
                            options.ClientId = secretStore.GetSecret("LinkedIn:ClientId");
                            options.ClientSecret = secretStore.GetSecret("LinkedIn:ClientSecret");
                        });
                        break;
                    case ExternalProviderTypeEnum.Twitter:
                        services.AddAuthentication().AddTwitter(options =>
                        {
                            options.ConsumerKey = secretStore.GetSecret("Twitter:ClientId");
                            options.ConsumerSecret = secretStore.GetSecret("Twitter:ClientSecret");
                        });
                        break;
                    case ExternalProviderTypeEnum.OAuth2:
                        services.AddAuthentication().AddOAuth(provider.Scheme!, options =>
                        {
                            options.ClientId = secretStore.GetSecret("OAuth2.ClientId") ?? "myOath";
                            options.ClientSecret = secretStore.GetSecret("OAuth2.ClientSecret") ?? "myOath";
                            options.AuthorizationEndpoint = provider.AuthorizationEndpoint;
                            options.TokenEndpoint = provider.TokenEndpoint;
                            options.UserInformationEndpoint = provider.UserInfoEndpoint;
                            options.CallbackPath = provider.CallbackPath ?? $"/signin-{provider.Scheme}";
                            options.SignInScheme = IdentityConstants.ExternalScheme;

                            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

                            options.Events = new OAuthEvents
                            {
                                OnCreatingTicket = async ctx =>
                                {
                                    var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                                    var resp = await ctx.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ctx.HttpContext.RequestAborted);
                                    resp.EnsureSuccessStatusCode();
                                    var user = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                                    ctx.RunClaimActions(user.RootElement);
                                }
                            };
                        });
                        break;
                        //case ExternalProviderTypeEnum.OpenIdConnect:
                        //    services.AddAuthentication().AddOpenIdConnect(provider.Scheme!, options =>
                        //    {
                        //        options.Authority = provider.MetadataJson; // or metadata URL
                        //        options.ClientId = provider.ClientId ?? secretStore.GetSecret("OpenIdConnect.ClientId");
                        //        options.ClientSecret = secretStore.GetSecret("OpenIdConnect.ClientSecret");
                        //        options.CallbackPath = provider.CallbackPath != "" ? provider.CallbackPath : $"/signin-{provider.Scheme}";
                        //        options.SignInScheme = IdentityConstants.ExternalScheme;
                        //        options.ResponseType = "code";
                        //        // ...map scopes/claims
                        //    });
                        //    break;
                        // ... برای LinkedIn, Twitter, ...
                }


            }
        }
        return services;
    }


}