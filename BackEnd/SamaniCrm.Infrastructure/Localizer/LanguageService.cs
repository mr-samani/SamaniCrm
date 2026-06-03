using Duende.IdentityServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Localizer
{
    public class LanguageService : ILanguageService
    {
        private readonly ICacheService _cacheService;
        private readonly MasterDbContext _dbContext;

        private readonly IDbContextFactory<MasterDbContext> _dbFactory;
        private readonly LocalizationMemoryCache _cache;


        public LanguageService(
            ICacheService cacheService,
            MasterDbContext dbContext,
            IDbContextFactory<MasterDbContext> dbFactory,
            LocalizationMemoryCache cache)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
            _dbFactory = dbFactory;
            _cache = cache;
        }


        /// <summary>
        /// دریافت لیست زبان های فعال
        /// (در صورت وجود دریافت از کش)
        /// </summary>
        /// <returns></returns>
        public async Task<List<LanguageDTO>> GetAllActiveLanguages()
        {
            List<LanguageDTO>? languageList = await _cacheService.GetAsync<List<LanguageDTO>>(CacheKeys.LanguageList);
            if (languageList == null)
            {
                languageList = await _dbContext.Languages
                                         .Where(x => x.IsActive)
                             .Select(s => new LanguageDTO()
                             {
                                 Name = s.Name,
                                 Culture = s.Culture,
                                 Flag = s.Flag,
                                 IsRtl = s.IsRtl,
                                 IsDefault = s.IsDefault
                             }).ToListAsync();

                await _cacheService.SetAsync(CacheKeys.LanguageList, languageList, TimeSpan.FromHours(8));
            }
            return languageList;
        }

        public async Task<List<LanguageDTO>> GetAllLanguagesForAdmin()
        {
            var languageList = await _dbContext.Languages
             .Select(s => new LanguageDTO()
             {
                 Name = s.Name,
                 Culture = s.Culture,
                 Flag = s.Flag,
                 IsRtl = s.IsRtl,
                 IsDefault = s.IsDefault,
                 IsActive = s.IsActive,
             }).ToListAsync();
            return languageList;
        }



        public async Task<string> GetAsync(string key, string culture)
        {
            Dictionary<string, string?> all = await GetAllLocalizatonsAsync(culture);
            return all != null && all.TryGetValue(key, out var value) ? (value ?? key) : key;
        }


        public async Task<Dictionary<string, string?>> GetAllLocalizatonsAsync(string culture)
        {
            //var result = await _dbContext.Languages
            //    .Where(x => x.Culture == culture)
            //    .SelectMany(x => x.Localizations!)
            //    .ToDictionaryAsync(x => x.Key, x => x.Value);


            var cacheKey = CacheKeys.GetLocalizationCacheKey(culture);
            var cached = await _cacheService.GetAsync<Dictionary<string, string?>>(cacheKey);
            if (cached is not null)
                return cached;

            Dictionary<string, string?> items = await _dbContext.Localizations
                .Where(x => x.Culture == culture)
                .ToDictionaryAsync(x => x.Key, x => x.Value);

            await _cacheService.SetAsync(cacheKey, items, TimeSpan.FromHours(12));

            return items;
        }




        public async Task PreloadAllLocalizationsAsync()
        {
            var db = _dbFactory.CreateDbContext();

            var data = await db.Localizations
                .GroupBy(x => x.Culture)
                .ToListAsync();

            foreach (var group in data)
            {
                var dict = group
                    .GroupBy(x => x.Key) // گروه‌بندی بر اساس کلید
                    .Select(g => g.First()) // فقط اولین رکورد هر کلید
                    .ToDictionary(x => x.Key, x => x.Value);
                _cache.SetCulture(group.Key, dict!);
            }
        }


    }
}
