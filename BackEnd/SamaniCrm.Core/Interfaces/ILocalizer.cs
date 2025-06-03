using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Interfaces;
public interface ILocalizer
{
    /// <summary>
    /// زبان فعلی (مثل fa-IR یا en-US)
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// دریافت مقدار کلید ساده (بدون پارامتر)
    /// </summary>
    /// <param name="key">کلید زبان</param>
    /// <returns>مقدار ترجمه‌شده</returns>
    string this[string key] { get; }

    /// <summary>
    /// دریافت مقدار کلید با پارامترهای فرمت (string.Format)
    /// </summary>
    /// <param name="key">کلید زبان</param>
    /// <param name="args">مقادیر جایگزین</param>
    /// <returns>مقدار ترجمه‌شده با جایگذاری پارامترها</returns>
    string this[string key, params object[] args] { get; }



    Dictionary<string, string>? GetAllStrings(bool includeParentCultures);


}

