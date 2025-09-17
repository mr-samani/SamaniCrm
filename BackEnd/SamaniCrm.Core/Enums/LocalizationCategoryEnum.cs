using System.ComponentModel;

namespace SamaniCrm.Core.Shared.Enums
;

public enum LocalizationCategoryEnum
{
    [Description("Role")]
    Role = 0,
    [Description("Permission")]
    Permission,
    [Description("Backend")]
    Backend,
    [Description("Frontend")]
    Frontend,
    [Description("Enum")]
    Enum,
    [Description("App")]
    App,
    [Description("Other")]
    Other
}
