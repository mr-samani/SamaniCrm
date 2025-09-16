
using SamaniCrm.Domain.Entities;
using System.ComponentModel;

namespace SamaniCrm.Application.DTOs;

public class UserSettingDto
{
    public Guid UserId;

    public string Secret { get; set; } = string.Empty;

    public bool EnableTwoFactor { get; set; }
    public TwoFactorTypeEnum TwoFactorType { get; set; }

    [Description("Is Verified Email,Mobile,AuthApp")]
    public bool IsVerified { get; set; }
}

 