using System.ComponentModel;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities;

public class UserSetting
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Secret { get; set; } = string.Empty;

    public bool EnableTwoFactor { get; set; }
    public TwoFactorTypeEnum TwoFactorType { get; set; }

    public int AttemptCount { get; set; }

    public DateTime LastAttemptAt { get; set; }

    [Description("Is Verified Email,Mobile,AuthApp")]
    public bool IsVerified { get; set; }


}


