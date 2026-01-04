using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.ExternalLogin;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SamaniCrm.Api.Controllers
{

    public class ExternalAuthController : ApiBaseController
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalAuthController> _logger;
        private readonly IIdentityService _identityService;

        public ExternalAuthController(
            IApplicationDbContext context,
            IConfiguration configuration,
            ILogger<ExternalAuthController> logger,
            IIdentityService identityService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _identityService = identityService;
        }



        [HttpGet("login/{providerName}")]
        public async Task<IActionResult> login(string providerName)
        {
            var frontendUrl = _configuration["FrontendUrl"] ?? "";
            if (frontendUrl.EndsWith("/") == false)
            {
                frontendUrl += "/";
            }
            var backendUrl = _configuration["BackendUrl"] ?? "";
            if (backendUrl.EndsWith("/") == false)
            {
                backendUrl += "/";
            }
            var provider = await _context.ExternalProviders
                  .FirstOrDefaultAsync(x => x.Name.ToLower() == providerName.ToLower() && x.IsActive);

            if (provider == null)
            {
                return Redirect($"{frontendUrl}account/login?error=provider_not_found");
            }

            var state = PkceHelper.GenerateState();
            var nonce = PkceHelper.GenerateNonce();

            var codeVerifier = PkceHelper.GenerateCodeVerifier();
            var codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);

            var redirectUri = $"{backendUrl}api/externalauth/callback/{provider.Name}";
            // حتماً code_verifier را ذخیره کن
            HttpContext.Session.SetString("pkce_code_verifier", codeVerifier);

            var authorizeUrl =
                $"{provider.AuthorizationEndpoint}" +
                $"?client_id={provider.ClientId}" +
                $"&response_type={provider.ResponseType}" +
                $"&scope={provider.Scopes}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&state={state}" +
                $"&nonce={nonce}" +
                $"&code_challenge={codeChallenge}" +
                $"&code_challenge_method=S256";

            return Redirect(authorizeUrl);

        }




        //https://localhost:44343/api/externalauth/callback/samaniauth?
        //state=b5fc83c9526e9bc400b024f098f74f37119d79f41c4971125cb166cef694bec6&
        //session_state=78dbe713-fc8b-2183-5534-4c9e7237d428&
        //iss=https%3A%2F%2Flocalhost%3A8443%2Frealms%2Fmaster&
        //code=b855ef16-3856-e32b-d149-1c5abe9a28a0.78dbe713-fc8b-2183-5534-4c9e7237d428.31befbc1-4fa2-4673-b56d-314cfebfb407

        [HttpGet("callback/{providerName}")]
        public async Task<IActionResult> Callback(
            string providerName,
            [FromQuery] string? code,
            [FromQuery] string? id_token,
            [FromQuery] string? access_token,
            [FromQuery] string? token_type,
            [FromQuery] string? state,
            [FromQuery] string? error,
            [FromQuery] string? error_description,
            CancellationToken cancellationToken)
        {
            var frontendUrl = _configuration["FrontendUrl"] ?? "";
            if (frontendUrl.EndsWith("/") == false)
            {
                frontendUrl += "/";
            }
            try
            {

                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogError($"External auth error: {error} - {error_description}");
                    return Redirect($"{frontendUrl}account/login?error={Uri.EscapeDataString(error_description ?? error)}");
                }

                var provider = await _context.ExternalProviders
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == providerName.ToLower() && x.IsActive);

                if (provider == null)
                {
                    return Redirect($"{frontendUrl}account/login?error=provider_not_found");
                }



                //return Redirect($"{frontendUrl}account/login?error=invalid_id_token");
                //return Redirect($"{frontendUrl}account/login?error=missing_tokens");
                //return Redirect($"{frontendUrl}account/login?error=token_exchange_failed");



                // TODO :code = idTokenFinal , code , accessTokenFinal
                var codeVerifier = HttpContext.Session.GetString("pkce_code_verifier");

                var req = new ExternalLoginCallbackCommand(code, providerName, codeVerifier);
                var loginResult = await _identityService.ExternalSignInAsync(req, cancellationToken);

                if (loginResult == null)
                {
                    return Redirect($"{frontendUrl}account/login?error=user_creation_failed");
                }
                if (loginResult.User == null)
                {
                    return Redirect($"{frontendUrl}account/login?error=no_userinfo");
                }
                if (string.IsNullOrEmpty(loginResult.User.Email))
                {
                    return Redirect($"{frontendUrl}account/login?error=userinfo_failed");
                }



                // Redirect to frontend with tokens
                var redirectUrl = $"{frontendUrl}account/callback?token={loginResult.AccessToken}&refreshToken={loginResult.RefreshToken}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in external auth callback");

                return Redirect($"{frontendUrl}account/login?error={Uri.EscapeDataString(ex.Message) ?? "unexpected_error"}");
            }
        }


    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }


}

