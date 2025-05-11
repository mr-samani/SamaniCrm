using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Queries.Role
{
    public class GetRoleQuery : IRequest<IList<RoleDTO>>
    {

    }

    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, IList<RoleDTO>>
    {
        private readonly IIdentityService _identityService;

        public GetRoleQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<IList<RoleDTO>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            var roles = await _identityService.GetRolesAsync();
            return roles.Select(role => new RoleDTO()
            {
                Id = role.id,
                RoleName = role.roleName,
                DisplayName = "Role:" + role.roleName,
            }).ToList();
        }
    }
}
