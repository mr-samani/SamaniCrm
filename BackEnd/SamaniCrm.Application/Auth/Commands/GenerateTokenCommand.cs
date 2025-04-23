using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Auth.Commands
{
    public record GenerateTokenCommand(IUser User) : IRequest<TokenResponseDto>;

}
