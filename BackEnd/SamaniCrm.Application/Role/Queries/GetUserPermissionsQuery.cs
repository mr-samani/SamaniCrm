using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Role.Queries
{
    public record GetUserPermissionsQuery(Guid UserId) : IRequest<List<string>>;

    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<string>>
    {
        private readonly IRolePermissionService _rolePermissionService;

        public GetUserPermissionsQueryHandler(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _rolePermissionService.GetPermissionsForUserAsync(request.UserId);


            return permissions;
        }
    }

}
