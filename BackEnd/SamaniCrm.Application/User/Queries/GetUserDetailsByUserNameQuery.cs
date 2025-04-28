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
    public class GetUserDetailsByUserNameQuery : IRequest<UserDetailsResponseDTO>
    {
        public string UserName { get; set; }
    }

    public class GetUserDetailsByUserNameQueryHandler : IRequestHandler<GetUserDetailsByUserNameQuery, UserDetailsResponseDTO>
    {
        private readonly IIdentityService _identityService;

        public GetUserDetailsByUserNameQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<UserDetailsResponseDTO> Handle(GetUserDetailsByUserNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _identityService.GetUserDetailsByUserNameAsync(request.UserName);
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
                Address= result.user.Address,
                PhoneNumber = result.user.PhoneNumber,
                CreationTime= result.user.CreationTime,
                Roles = result.roles.ToArray(),
            };
        }
    }
}
