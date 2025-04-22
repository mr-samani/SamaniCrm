using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Application.Auth.Commands
{
    public record GenerateTokenCommand(ApplicationUser User) : IRequest<TokenResponseDto>;

}
