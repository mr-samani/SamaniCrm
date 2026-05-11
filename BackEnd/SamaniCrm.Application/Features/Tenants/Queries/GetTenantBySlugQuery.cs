using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.Interfaces.Tenant;


namespace SamaniCrm.Application.Features.Tenants.Queries;

public record GetTenantBySlugQuery(string Slug) : IRequest<TenantDto>;
public class GetTenantBySlugQueryHandler : IRequestHandler<GetTenantBySlugQuery, TenantDto>
{
    private readonly ITenantRepository _repository;

    public GetTenantBySlugQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<TenantDto> Handle(GetTenantBySlugQuery request, CancellationToken cancellation)
    {
        var tenant = await _repository.GetBySlugAsync(request.Slug, cancellation)
            ?? throw new NotFoundException("Tenant not found");
        return tenant;
    }

}

