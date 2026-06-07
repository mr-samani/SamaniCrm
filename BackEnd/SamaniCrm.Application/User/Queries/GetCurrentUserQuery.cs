using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.User.Queries
{
    public record GetCurrentUserQuery : IRequest<UserDTO>;


    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDTO>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IIdentityService _identityService;
        private readonly IUserPermissionService _userPermissionService;

        public GetCurrentUserQueryHandler(
            ICurrentUserService currentUser,
            IIdentityService identityService,
            IUserPermissionService userPermissionService)
        {
            _currentUser = currentUser;
            _identityService = identityService;
            _userPermissionService = userPermissionService;
        }

        public async Task<UserDTO> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
            {
                throw new UnAuthenticateException();
            }
            var currentUserId = (Guid)_currentUser.UserId;

            var result = await _identityService.GetUserDetailsAsync(currentUserId, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException("User not found.");
            }
            var permissions = await _userPermissionService.GetUserPermissionsAsync(currentUserId, cancellationToken);
            result.Permissions = permissions;
            return result;
        }
    }
}
