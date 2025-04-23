using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Queries
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, string[]>
    {
        private readonly UserManager<IUser> _userManager;

        public GetUserRolesQueryHandler(UserManager<IUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string[]> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            return (await _userManager.GetRolesAsync(request.User)).ToArray();
        }
    }
}
