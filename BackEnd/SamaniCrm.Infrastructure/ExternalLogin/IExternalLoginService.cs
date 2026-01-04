using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Infrastructure.ExternalLogin;

public interface IExternalLoginService
{
    Task<ExternalLoginResult> ExchangeCodeAsync(ExternalProviderTypeEnum provider, string code, string tokenEndpoint, string clientId, string clientSecret, string codeVerifier, string redirectUri, CancellationToken cancellationToken);

}
