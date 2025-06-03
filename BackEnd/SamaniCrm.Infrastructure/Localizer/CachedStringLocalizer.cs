using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Localizer;
public class CachedStringLocalizer : ILocalizer
{ 
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LocalizationMemoryCache _cache;

    public CachedStringLocalizer(IHttpContextAccessor httpContextAccessor, LocalizationMemoryCache cache)
    {
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }


    public string CurrentLanguage => GetCulture();


    private string GetCulture()
    {
        return _httpContextAccessor.HttpContext?.Items["lang"]?.ToString()
               ?? CultureInfo.CurrentUICulture.Name ?? AppConsts.DefaultLanguage;
    }



    public string this[string name]
    {
       
        get
        {
            var culture = GetCulture();
            var value = _cache.Get(culture, name);
            return  value ?? name;
        }
    }

    public string this[string name, params object[] arguments]
    {
        get
        {
            var baseString = this[name]; 
            return  string.Format(baseString, arguments);
        }
    }


    public Dictionary<string, string>? GetAllStrings(bool includeParentCultures)
    {
        var culture = GetCulture();
        Dictionary<string, string>? dict = _cache.GetAllForCulture(culture);
        return dict;//.Select(x => new LocalizedString(x.Key, x.Value, false));
    }
}
