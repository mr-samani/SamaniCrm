using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SamaniCrm.Application.Auth.Commands;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommand: IRequest<LoginResult>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }



}
