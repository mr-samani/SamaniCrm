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
    public class GetUserDetailsByUserNameQuery : IRequest<UserDTO>
    {
        public string UserName { get; set; }
    }

    public class GetUserDetailsByUserNameQueryHandler : IRequestHandler<GetUserDetailsByUserNameQuery, UserDTO>
    {
        private readonly IIdentityService _identityService;

        public GetUserDetailsByUserNameQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<UserDTO> Handle(GetUserDetailsByUserNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _identityService.GetUserDetailsByUserNameAsync(request.UserName);
            return new UserDTO()
            {
                Id = result.Id,
                UserName = result.UserName,
                FirstName = result.FirstName,
                LastName = result.LastName,
                ProfilePicture = result.ProfilePicture,
                Lang = result.Lang,
                Email = result.Email,
                FullName = result.FullName,
                Address= result.Address,
                PhoneNumber = result.PhoneNumber,
                CreationTime= result.CreationTime,
                Roles = result.Roles,
            };
        }
    }
}
