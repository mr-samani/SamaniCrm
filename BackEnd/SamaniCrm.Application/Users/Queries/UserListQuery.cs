using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;

namespace SamaniCrm.Application.Users.Queries
{
    public class UserListQuery : PaginationRequest, IRequest<PaginatedResult<UserDto>>
    {
        public string? Filter { get; set; }
    }


}
