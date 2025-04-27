using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Captcha.Command
{
    public class VerifyCaptchaCommand : IRequest<bool>
    {
        public required string Key { get; set; }
        public required string CaptchaText { get; set; }
    }
}
