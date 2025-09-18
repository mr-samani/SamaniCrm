using Duende.IdentityServer.Endpoints.Results;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Core.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
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




    private void getGitHubUserData()
    {

    }

}
