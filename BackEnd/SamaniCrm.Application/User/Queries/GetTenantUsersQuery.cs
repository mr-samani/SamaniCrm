using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Queries.User;

public class GetTenantUsersQuery : PaginationRequest, IRequest<PaginatedResult<TenantUserDTO>>
{
    public Guid TenantId { get; set; }
    public string? Filter { get; set; }
}
public class GetTenantUsersQueryHandler : IRequestHandler<GetTenantUsersQuery, PaginatedResult<TenantUserDTO>>
{
    private readonly IIdentityService _identityService;

    public GetTenantUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<PaginatedResult<TenantUserDTO>> Handle(GetTenantUsersQuery request, CancellationToken cancellationToken)
    {
        PaginatedResult<TenantUserDTO> users = await _identityService.GetTenantUsersAsync(request, cancellationToken);
        return users;
    }
}
