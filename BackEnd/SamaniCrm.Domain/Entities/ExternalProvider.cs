using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ExternalProvider : IAuditableEntity
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
    public string ClientId { get; set; } = "";

    [MaxLength(500)]
    public string ClientSecret { get; set; } = ""; // Encrypted in production


    [MaxLength(255)]
    public required string AuthorizationEndpoint { get; set; }
    [MaxLength(255)]
    public required string TokenEndpoint { get; set; }
    [MaxLength(255)]
    public required string UserInfoEndpoint { get; set; }
    [MaxLength(255)]
    public string? CallbackPath { get; set; } // e.g. /signin-google-1

    [MaxLength(255)]
    public string? LogoutEndpoint { get; set; }

    public string? MetadataJson { get; set; } // optional custom mappings

    [MaxLength(500)]
    public required string Scopes { get; set; } // space/comma separated

    [MaxLength(50)]
    public string ResponseType { get; set; } = "code"; // code, id_token, token, code id_token, code token, id_token token, code id_token token

    [MaxLength(50)]
    public string ResponseMode { get; set; } = "query"; // query, fragment, form_post

    public bool UsePkce { get; set; } = true;

    public bool IsActive { get; set; } = false;



    // Implementing IAuditableEntity properties
    public DateTime CreationTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedTime { get; set; }
    public string? LastModifiedBy { get; set; }
}
