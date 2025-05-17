using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Infrastructure.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;

        public LanguageService(ICacheService cacheService, ApplicationDbContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
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

        public async Task<Dictionary<string, string>> GetAllValuesAsync(string culture)
        {
            var result = await _dbContext.Languages
                .Where(x => x.Culture == culture)
                .SelectMany(x => x.Localizations!)
                .ToDictionaryAsync(x => x.Key, x => x.Value);
            return result!;
        }
    }
}
