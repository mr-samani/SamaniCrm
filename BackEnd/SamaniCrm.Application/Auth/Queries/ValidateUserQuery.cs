using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Auth.Queries
{
    public record ValidateUserQuery(string Username, string Password) : IRequest<ApplicationUser?>;

}
