using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;
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

        [HttpGet("callback/{providerName}")]
        public async Task<IActionResult> Callback(
            string providerName,
            [FromQuery] string code,
            [FromQuery] string id_token,
            [FromQuery] string access_token,
            [FromQuery] string token_type,
            [FromQuery] string state,
            [FromQuery] string error,
            [FromQuery] string error_description,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogError($"External auth error: {error} - {error_description}");
                    return Redirect($"{_configuration["FrontendUrl"]}/login?error={Uri.EscapeDataString(error_description ?? error)}");
                }

                var provider = await _context.ExternalProviders
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == providerName.ToLower() && x.IsActive);

                if (provider == null)
                {
                    return Redirect($"{_configuration["FrontendUrl"]}/login?error=provider_not_found");
                }



                //return Redirect($"{_configuration["FrontendUrl"]}/login?error=invalid_id_token");
                //return Redirect($"{_configuration["FrontendUrl"]}/login?error=missing_tokens");
                //return Redirect($"{_configuration["FrontendUrl"]}/login?error=token_exchange_failed");



                // TODO :code = idTokenFinal , code , accessTokenFinal

                var req = new ExternalLoginCallbackCommand(code, providerName);
                var loginResult = await _identityService.ExternalSignInAsync(req, cancellationToken);

                if (loginResult == null)
                {
                    return Redirect($"{_configuration["FrontendUrl"]}/login?error=user_creation_failed");
                }
                if (loginResult.User == null)
                {
                    return Redirect($"{_configuration["FrontendUrl"]}/login?error=no_userinfo");
                }
                if (string.IsNullOrEmpty(loginResult.User.Email))
                {
                    return Redirect($"{_configuration["FrontendUrl"]}/login?error=userinfo_failed");
                }

               

                // Redirect to frontend with tokens
                var redirectUrl = $"{_configuration["FrontendUrl"]}/auth/callback?token={loginResult.AccessToken}&refreshToken={loginResult.RefreshToken}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in external auth callback");
                return Redirect($"{_configuration["FrontendUrl"]}/login?error=unexpected_error");
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

