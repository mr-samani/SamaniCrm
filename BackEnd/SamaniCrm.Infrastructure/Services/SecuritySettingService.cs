using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Services
{
    public class SecuritySettingService : ISecuritySettingService
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICacheService _cacheService;

        public SecuritySettingService(IApplicationDbContext applicationDbContext, ICacheService cacheService)
        {
            _applicationDbContext = applicationDbContext;
            _cacheService = cacheService;
        }

        public async Task<SecuritySettingDTO> GetSettingsAsync(CancellationToken cancellationToken)
        {
            PasswordComplexityDTO? data = await _cacheService.GetAsync<PasswordComplexityDTO>(CacheKeys.SecuritySettings);
            if (data == null)
            {
                data = await _applicationDbContext.SecuritySettings.Select(s => new PasswordComplexityDTO
                {
                    RequiredLength = s.RequiredLength,
                    RequireDigit = s.RequireDigit,
                    RequireLowercase = s.RequireLowercase,
                    RequireUppercase = s.RequireUppercase,
                    RequireNonAlphanumeric = s.RequireNonAlphanumeric
                }).FirstOrDefaultAsync(cancellationToken);
                await _cacheService.SetAsync(CacheKeys.SecuritySettings, data, TimeSpan.FromHours(4));
            }
            return new SecuritySettingDTO
            {
                PasswordComplexity = data ?? default!
            };
        }

        public async Task<bool> SetSettingsAsync(SecuritySettingDTO input, CancellationToken cancellationToken)
        {
            var data = await _applicationDbContext.SecuritySettings.FirstAsync();
            int result;
            if (data == null)
            {
                await _applicationDbContext.SecuritySettings.AddAsync(new SecuritySetting()
                {
                    RequireDigit = input.PasswordComplexity.RequireDigit,
                    RequiredLength = input.PasswordComplexity.RequiredLength,
                    RequireLowercase = input.PasswordComplexity.RequireLowercase,
                    RequireNonAlphanumeric = input.PasswordComplexity.RequireUppercase,
                    RequireUppercase = input.PasswordComplexity.RequireUppercase
                });
            }
            else
            {
                data.RequireDigit = input.PasswordComplexity.RequireDigit;
                data.RequiredLength = input.PasswordComplexity.RequiredLength;
                data.RequireLowercase = input.PasswordComplexity.RequireLowercase;
                data.RequireNonAlphanumeric = input.PasswordComplexity.RequireNonAlphanumeric;
                data.RequireUppercase = input.PasswordComplexity.RequireUppercase;
                _applicationDbContext.SecuritySettings.Update(data);
            }
            result = await _applicationDbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.SecuritySettings);
            return result > 0;

        }
    }
}
