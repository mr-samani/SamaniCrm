using Duende.IdentityServer.Endpoints.Results;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.ExternalLogin;
public class ExternalLoginService : IExternalLoginService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalLoginService> _logger;

    public ExternalLoginService(HttpClient httpClient, ILogger<ExternalLoginService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ExternalLoginResult> ExchangeCodeAsync(ExternalProviderTypeEnum provider, string code, string tokenEndpoint, string clientId, string clientSecret, string redirectUri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            })
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("Sending token request to {TokenEndpoint} for provider {ClientId}", tokenEndpoint, clientId);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("Token response from {TokenEndpoint}: {Content}", tokenEndpoint, content);

        var result = await ParseTokenResponseAsync(response, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ExternalLoginException(result.ErrorDescription ?? "Error on login with external: " + provider);
        }

        response.EnsureSuccessStatusCode();

        string access_token = result.AccessToken;
        string idToken = result.IdToken;

        var output = new ExternalLoginResult();
        switch (provider)
        {
            case ExternalProviderTypeEnum.Microsoft:
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(idToken ?? access_token);
                output.Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                            ?? jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                output.Name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? output.Email;
                output.Image = jwt.Claims.FirstOrDefault(c => c.Type == "avatar")?.Value ?? "";
                output.UserName = output.Email;
                break;
            case ExternalProviderTypeEnum.GitHub:
                var githubResult = await GitHub.GetGitHubUserAsync(_httpClient, access_token, cancellationToken);
                output.Email = githubResult.email;
                output.Name = githubResult.user.Name ?? githubResult.user.Login;
                output.UserName = githubResult.user.Login;
                break;
            case ExternalProviderTypeEnum.LinkedIn:
                var linkdinResult = await Linkdin.GetLinkdinUserAsync(_httpClient, access_token, cancellationToken);
                output.Email = linkdinResult.email;
                output.Name = linkdinResult.user.Name ?? linkdinResult.user.Login;
                output.UserName = linkdinResult.user.Login;
                break;
            case ExternalProviderTypeEnum.OpenIdConnect:
                var claims = ValidateIdToken(idToken, null);
                if (claims == null) return null;
                output.UserName = claims.ContainsKey("sub") ? claims["sub"].ToString() : null;
                output.Email = claims.ContainsKey("email") ? claims["email"].ToString() : null;
                output.Name = claims.ContainsKey("name") ? claims["name"].ToString() : null;
                output.GivenName = claims.ContainsKey("given_name") ? claims["given_name"].ToString() : null;
                output.LastName = claims.ContainsKey("family_name") ? claims["family_name"].ToString() : null;
                output.Image = claims.ContainsKey("picture") ? claims["picture"].ToString() : null;
                break;
        }




        if (output.Email == null)
            throw new ValidationException("Email not found in external login");

        return output;
    }
    private async Task<TokenResponse> ParseTokenResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            // تلاش برای JSON
            var token = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (token != null) return token;
        }
        catch (JsonException)
        {
            // JSON نبود → fallback
        }

        // اگر JSON نبود → احتمالاً x-www-form-urlencoded
        var dict = System.Web.HttpUtility.ParseQueryString(content);
        var fallback = new TokenResponse
        {
            AccessToken = dict["access_token"] ?? "",
            RefreshToken = dict["refresh_token"],
            IdToken = dict["id_token"],
            TokenType = dict["token_type"],
            Scope = dict["scope"],
            ExpiresIn = int.TryParse(dict["expires_in"], out var exp) ? exp : null,
            Error = dict["error"],
            ErrorDescription = dict["error_description"]
        };

        return fallback;
    }

    // Validate ID Token (basic validation - in production use proper JWT library)
    private Dictionary<string, object> ValidateIdToken(string idToken, ExternalProvider provider)
    {
        try
        {
            // Decode JWT (without signature verification for now)
            var parts = idToken.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            var json = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            // TODO: In production, verify signature using provider's public keys (JWKS)
            // TODO: Verify iss, aud, exp, nonce claims

            return claims;
        }
        catch
        {
            return null;
        }
    }
    private byte[] Base64UrlDecode(string input)
    {
        var output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2: output += "=="; break;
            case 3: output += "="; break;
        }
        return Convert.FromBase64String(output);
    }

    private void getGitHubUserData()
    {

    }

}
