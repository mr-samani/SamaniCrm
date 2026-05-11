using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Core.Shared.Interfaces.Tenant;


namespace SamaniCrm.Application.Features.Tenants.Queries;

public record GetTenantByIdQuery(Guid Id) : IRequest<TenantDto>;
public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly ITenantRepository _repository;

    public GetTenantByIdQueryHandler(ITenantRepository repository) => _repository = repository;

    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellation)
    {
        var tenant = await _repository.GetByIdAsync(request.Id,cancellation)
            ?? throw new NotFoundException("Tenant not found");
        return (tenant);
    }

}

