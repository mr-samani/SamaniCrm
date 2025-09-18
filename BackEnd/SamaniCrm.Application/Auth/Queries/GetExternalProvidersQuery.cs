using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Queries;

public record GetExternalProvidersQuery(bool isActive = true) : IRequest<List<ExternalProviderDto>>;

public class GetExternalProvidersHandler : IRequestHandler<GetExternalProvidersQuery, List<ExternalProviderDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ISecretStore _secretStore;
    public GetExternalProvidersHandler(IApplicationDbContext db, ISecretStore secretStore)
    {
        _db = db;
        _secretStore = secretStore;
    }

    public async Task<List<ExternalProviderDto>> Handle(GetExternalProvidersQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ExternalProviders.AsQueryable();
        if (request.isActive == true)
        {
            query = query.Where(p => p.IsActive == true);
        }

        var result = await query
           .Select(p => new ExternalProviderDto()
           {
               Id = p.Id,
               Name = p.Name,
               DisplayName = p.DisplayName,
               ProviderType = p.ProviderType,
               AuthorizationEndpoint = p.AuthorizationEndpoint,
               ClientId = _secretStore.GetSecret(p.Name + ":ClientId"),
               Scopes = p.Scopes,
               IsActive = p.IsActive
           })
           .ToListAsync(cancellationToken);
        return result;
    }
}
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
}
