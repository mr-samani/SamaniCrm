using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using MediatR;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Application.Queries.User
{
    public class GetUserDetailsByUserNameQuery : IRequest<UserDTO>
    {
        public required string UserName { get; set; }
        public Guid? TenantId { get; set; }
    }

    public class GetUserDetailsByUserNameQueryHandler : IRequestHandler<GetUserDetailsByUserNameQuery, UserDTO>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentTenant _currentTenant;

        public GetUserDetailsByUserNameQueryHandler(IIdentityService identityService, ICurrentTenant currentTenant)
        {
            _identityService = identityService;
            _currentTenant = currentTenant;
        }
        public async Task<UserDTO> Handle(GetUserDetailsByUserNameQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = _currentTenant.TenantId;
            UserDTO result = await _identityService.GetUserDetailsByUserNameAsync(request.UserName, request.TenantId,cancellationToken);
            return result;
        }
    }
}
