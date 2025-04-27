using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Queries.User;

namespace SamaniCrm.Application.Auth.Commands
{
    public class LoginCommand: IRequest<LoginResult>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }

        public InputCaptchaDTO? captcha { get; set; }
    }

}
