using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.DTOs
{
    public class InputCaptchaDTO
    {
        public required string CaptchaKey { get; set; }
        public required string CaptchaText { get; set; }
    }
}
