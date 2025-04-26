using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Auth.Commands
{
    public record LoginCommand(string UserName,string Password):IRequest<LoginResult>;

    
}
