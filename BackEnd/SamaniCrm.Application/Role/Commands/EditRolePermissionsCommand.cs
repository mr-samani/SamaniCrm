using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Role.Commands
{
    public record EditRolePermissionsCommand(
        List<string> GrantedPermissions,
        string RoleName
        ) : IRequest<bool>;

    public class EditRolePermissionsCommandHandler : IRequestHandler<EditRolePermissionsCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public EditRolePermissionsCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(EditRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.UpdateRolePermissionsAsync(request, cancellationToken);
            return result;
        }
    }

}
