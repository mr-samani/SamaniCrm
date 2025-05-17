using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SamaniCrm.Core;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Infrastructure.Services;
public class Localizer : ILocalizer
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILanguageService _languageService;
    private readonly ICacheService _cacheService;

    public Localizer(
        ICacheService cacheService,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
    {
        _cacheService = cacheService;
        _languageService = languageService;
        _httpContextAccessor = httpContextAccessor;
    }

    private Dictionary<string, string>? _cache;



    private string LanguageCode
        => _httpContextAccessor.HttpContext?.Items["lang"]?.ToString() ?? AppConsts.DefaultLanguage;

    public string CurrentLanguage => LanguageCode;

    private async Task LoadCacheAsync()
    {
        if (_cache != null)
            return;

        var cacheKey = $"localization:{LanguageCode}";
        var dict = await _cacheService.GetAsync<Dictionary<string, string>>(cacheKey);
        if (dict == null)
        {
            dict = await _languageService.GetAllValuesAsync(LanguageCode);
            await _cacheService.SetAsync(cacheKey, dict, TimeSpan.FromHours(6));
        }

        _cache = dict;
    }

    public string this[string key]
    {
        get
        {
            LoadCacheAsync().GetAwaiter().GetResult(); // چون indexer async نیست
            return _cache!.TryGetValue(key, out var value)
                ? value
                : $"[{key}]";
        }
    }

    public string this[string key, params object[] args]
        => string.Format(this[key], args);
}
