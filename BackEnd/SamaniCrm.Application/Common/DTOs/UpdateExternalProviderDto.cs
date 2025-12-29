using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Application.Common.DTOs;

public class UpdateExternalProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Scheme { get; set; }
    public ExternalProviderTypeEnum ProviderType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string UserInfoEndpoint { get; set; }
    public string CallbackPath { get; set; }
    public string LogoutEndpoint { get; set; }
    public string MetadataJson { get; set; }
    public string Scopes { get; set; }
    public string ResponseType { get; set; }
    public string ResponseMode { get; set; }
    public bool UsePkce { get; set; }
}
