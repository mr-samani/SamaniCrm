using System.ComponentModel;

namespace SamaniCrm.Core.Shared.Enums
;

public enum MenuTargetEnum
{
    [Description("_self")]
    Self,

    [Description("_blank")]
    Blank,

    [Description("_parent")]
    Parent,

    [Description("_top")]
    Top
}
