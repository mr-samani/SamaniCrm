using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Common.DTOs;

namespace SamaniCrm.Application.Queries.User
{
    public class GetUserQuery : PaginationRequest, IRequest<PaginatedResult<UserDTO>>
    {
        public string? Filter { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, PaginatedResult<UserDTO>>
    {
        private readonly IIdentityService _identityService;

        public GetUserQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<PaginatedResult<UserDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<UserDTO> users = await _identityService.GetAllUsersAsync(request, cancellationToken);
            return users;
        }
    }
}
