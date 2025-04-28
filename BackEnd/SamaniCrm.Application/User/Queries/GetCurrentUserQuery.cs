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
    public record GetCurrentUserQuery : IRequest<UserResponseDTO>;


    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserResponseDTO>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<UserResponseDTO> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out Guid currentUserId))
            {
                throw new UnAuthenticateException();
            }

            var result = await _identityService.GetUserDetailsAsync(currentUserId);

            if (result.user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return result.user;
        }
    }
}
