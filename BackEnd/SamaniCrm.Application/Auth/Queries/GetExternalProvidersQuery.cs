using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Auth.Queries;

public record GetExternalProvidersQuery() : IRequest<List<ExternalProviderDto>>;

public class GetExternalProvidersHandler : IRequestHandler<GetExternalProvidersQuery, List<ExternalProviderDto>>
{
    private readonly AppDbContext _db;
    public GetExternalProvidersHandler(AppDbContext db) => _db = db;

    public async Task<List<ExternalProviderDto>> Handle(GetExternalProvidersQuery query, CancellationToken ct)
    {
        return await _db.ExternalProviderConfigs
            .Where(p => p.IsEnabled)
            .Select(p => new ExternalProviderDto(p.Name, p.Scheme))
            .ToListAsync(ct);
    }
}
public record ExternalProviderDto(string Name, string Scheme);
