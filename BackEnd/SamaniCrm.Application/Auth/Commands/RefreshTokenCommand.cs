using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponseDto>;

}
