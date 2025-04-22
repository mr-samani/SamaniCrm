using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Auth.Commands
{
    public record RevokeRefreshTokenCommand(string Token) : IRequest<bool>;

}
