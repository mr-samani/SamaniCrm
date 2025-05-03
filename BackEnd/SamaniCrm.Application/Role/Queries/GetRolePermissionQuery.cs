using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Role.Queries
{
    public record GetRolePermissionQuery(Guid roleId) : IRequest<List<RolePermissionsDTO>>;

    public class GetRolePermissionQueryHandler : IRequestHandler<GetRolePermissionQuery, List<RolePermissionsDTO>>
    {
        private readonly IRolePermissionService _rolePermissionService;

        public GetRolePermissionQueryHandler(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        public async Task<List<RolePermissionsDTO>> Handle(GetRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _rolePermissionService.GetRolePermissionsAsyc(request.roleId);
            return permissions;
        }
    }
}
