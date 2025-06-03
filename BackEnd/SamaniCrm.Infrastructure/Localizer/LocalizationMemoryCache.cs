using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Localizer;
public class LocalizationMemoryCache
{
    private readonly Dictionary<string, Dictionary<string, string>> _allCultures
        = new(StringComparer.OrdinalIgnoreCase);

    public void SetCulture(string culture, Dictionary<string, string> values)
    {
        _allCultures[culture] = values;
    }

    public string Get(string culture, string key)
    {
        if (_allCultures.TryGetValue(culture, out var dict))
        {
            if (dict.TryGetValue(key, out var val))
                return val;
        }

        return key; // fallback
    }

    public Dictionary<string, string>? GetAllForCulture(string culture)
    {
        _allCultures.TryGetValue(culture, out Dictionary<string, string>? dict);
        return dict;
    }
}
