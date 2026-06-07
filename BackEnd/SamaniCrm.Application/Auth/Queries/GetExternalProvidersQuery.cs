using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;
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
    private readonly IMasterDbContext _db;
    private readonly ISecretStore _secretStore;
    public GetExternalProvidersHandler(IMasterDbContext db, ISecretStore secretStore)
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
               ClientSecret = p.ClientSecret,
               DisplayName = p.DisplayName,
               ProviderType = p.ProviderType,
               TokenEndpoint = p.AuthorizationEndpoint,
               ClientId = p.ClientId != null ? p.ClientId : _secretStore.GetSecret(p.Name + ":ClientId"),
               Scopes = p.Scopes,
               ResponseType = p.ResponseType,
               ResponseMode = p.ResponseMode,
               UsePkce = p.UsePkce,
               IsActive = p.IsActive
           })
           .ToListAsync(cancellationToken);
        return result;
    }
}
