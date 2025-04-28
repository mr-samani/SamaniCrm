using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Queries.User
{
    public class GetUserDetailsQuery : IRequest<UserDetailsResponseDTO>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDetailsResponseDTO>
    {
        private readonly IIdentityService _identityService;

        public GetUserDetailsQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<UserDetailsResponseDTO> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _identityService.GetUserDetailsAsync(request.UserId);
            return new UserDetailsResponseDTO()
            {
                Id = result.user.Id,
                UserName = result.user.UserName,
                FirstName = result.user.FirstName,
                LastName = result.user.LastName,
                ProfilePicture = result.user.ProfilePicture,
                Lang = result.user.Lang,
                Email = result.user.Email,
                FullName = result.user.FullName,
                Roles = result.roles.ToArray(),
            };
        }
    }
}
