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
    public class GetUserDetailsQuery : IRequest<UserDTO>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDTO>
    {
        private readonly IIdentityService _identityService;

        public GetUserDetailsQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<UserDTO> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _identityService.GetUserDetailsAsync(request.UserId);
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
                Address = result.Address,
                PhoneNumber = result.PhoneNumber,
                CreationTime = result.CreationTime,
                Roles = result.Roles,
            };
        }
    }
}
