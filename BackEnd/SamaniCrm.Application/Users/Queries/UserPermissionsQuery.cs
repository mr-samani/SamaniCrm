using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Users.Queries
{
    public class UserPermissionsQuery : IRequest<Dictionary<string, bool>>
    {
    }
}
