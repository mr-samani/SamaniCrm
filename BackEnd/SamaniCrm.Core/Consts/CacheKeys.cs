using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Consts
{
    public abstract class CacheKeys
    {
        [Description("list of languages")]
        public const string LanguageList = "LanguageList";

        [Description("List user Permissions")]
        public const string UserPermissions_ = "UserPermissions_";

        [Description("Security settings: password complexity,...")]
        public const string SecuritySettings = "SecuritySettings";

        public static string GetLocalizationCacheKey(string culture) => $"localization:{culture}";

    }
}
