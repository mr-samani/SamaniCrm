using System.ComponentModel;

namespace SamaniCrm.Core.Shared.Enums
;

public enum ProductAttributeDataTypeEnum
{
    [Description("String")]
    String = 0,
    [Description("Int")]
    Int,
    [Description("Decimal")]
    Decimal,
    [Description("Bool")]
    Bool,
    [Description("Color")]
    Color
}
