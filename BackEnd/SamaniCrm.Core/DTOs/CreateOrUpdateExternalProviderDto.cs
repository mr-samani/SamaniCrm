using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Core.Shared.DTOs;

public class CreateOrUpdateExternalProviderDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Scheme { get; set; } = default!;
    public ExternalProviderTypeEnum ProviderType { get; set; }
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string AuthorizationEndpoint { get; set; } = default!;
    public string TokenEndpoint { get; set; } = default!;
    public string UserInfoEndpoint { get; set; } = default!;
    public string CallbackPath { get; set; } = default!;
    public string LogoutEndpoint { get; set; } = default!;
    public string MetadataJson { get; set; } = default!;
    public string Scopes { get; set; } = default!;
    public string ResponseType { get; set; } = default!;
    public string ResponseMode { get; set; } = default!;
    public bool UsePkce { get; set; }
}
