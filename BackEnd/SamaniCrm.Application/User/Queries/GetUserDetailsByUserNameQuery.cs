using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Application.Features.Tenants.Interfaces;

namespace SamaniCrm.Application.Queries.User
{
    public class GetUserDetailsByUserNameQuery : IRequest<UserDTO>
    {
        public required string UserName { get; set; }
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
            var result = await _identityService.GetUserDetailsByUserNameAsync(request.UserName, tenantId);
            return new UserDTO()
            {
                Id = result.Id,
                UserName = result.UserName,
                TenantId = result.TenantId,
                FirstName = result.FirstName,
                LastName = result.LastName,
                ProfilePicture = result.ProfilePicture,
                Lang = result.Lang,
                Email = result.Email,
                FullName = result.FullName,
                Address = result.Address,
                PhoneNumber = result.PhoneNumber,
                CreationTime = result.CreationTime,
                Roles = result.Roles,
                Permissions = []
            };
        }
    }
}
