using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<T> GetFlags<T>(this T enumValue) where T : Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (enumValue.HasFlag(value) && Convert.ToInt32(value) != 0)
            {
                yield return value;
            }
        }
    }
}