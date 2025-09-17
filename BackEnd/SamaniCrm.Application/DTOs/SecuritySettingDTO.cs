using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class SecuritySettingDto
    {
        public bool RequireCaptchaOnLogin { get; set; }

        public required PasswordComplexityDto PasswordComplexity { get; set; }

        public int LogginAttemptCountLimit { get; set; }
        public int LogginAttemptTimeSecondsLimit { get; set; }

    }
}
