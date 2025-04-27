using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Captcha.Queries
{
    public class GetCaptchaQuery : IRequest<CaptchaDto>
    {
    }
}
