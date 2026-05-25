using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class SecuritySetting : IMayHaveTenant
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }

    public int RequiredLength { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }


    public bool RequireCaptchaOnLogin { get; set; }


    public int LogginAttemptCountLimit { get; set; } = 10;

    public int LogginAttemptTimeSecondsLimit { get; set; } = 5 * 60;
}

