using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ExternalProvider
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; } // "Google", "MyCorpOIDC"
    public string DisplayName { get; set; } = default!; // "Google Login"

    [MaxLength(255)]
    public string? Scheme { get; set; } // "Google-1"
    [MaxLength(255)]
    public required ExternalProviderTypeEnum ProviderType { get; set; } // "OpenIdConnect" | "OAuth2"

    [MaxLength(255)]
    public required string AuthorizationEndpoint { get; set; }
    [MaxLength(255)]
    public required string TokenEndpoint { get; set; }
    [MaxLength(255)]
    public required string UserInfoEndpoint { get; set; }
    [MaxLength(255)]
    public string? CallbackPath { get; set; } // e.g. /signin-google-1
 
    public string? MetadataJson { get; set; } // optional custom mappings

    [MaxLength(500)]
    public required string Scopes { get; set; } // space/comma separated
    public bool IsActive { get; set; } = false;
}
