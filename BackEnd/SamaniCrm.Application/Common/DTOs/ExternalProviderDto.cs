using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.Common.DTOs;

public class ExternalProviderDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; } // "Google", "MyCorpOIDC"
    public string DisplayName { get; set; } = default!; // "Google Login"
    public required ExternalProviderTypeEnum ProviderType { get; set; } // "OpenIdConnect" | "OAuth2"

    public required string AuthorizationEndpoint { get; set; }
    public required string ClientId { get; set; }
    public required string Scopes { get; set; }

    public bool IsActive { get; set; } = false;


    public string ResponseType { get; set; } = "";
    public string ResponseMode { get; set; } = "";
    public bool UsePkce { get; set; }
}
