using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
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
        private readonly ICurrentUserService _currentUser;

        public SecuritySettingService(IApplicationDbContext applicationDbContext, ICacheService cacheService, ICurrentUserService currentUser)
        {
            _applicationDbContext = applicationDbContext;
            _cacheService = cacheService;
            _currentUser = currentUser;
        }

        public async Task<SecuritySettingDto> GetSettingsAsync(Guid? userId, CancellationToken cancellationToken)
        {
            UserSettingDto? userSetting = null;
            if (userId != null)
            {
                userSetting = await _cacheService.GetAsync<UserSettingDto>(CacheKeys.UserSetting_ + userId.ToString());
                if (userSetting == null)
                {
                    userSetting = await _applicationDbContext.UserSetting.Select(s => new UserSettingDto
                    {
                        UserId = s.UserId,
                        EnableTwoFactor = s.EnableTwoFactor,
                        TwoFactorType = s.TwoFactorType,
                        IsVerified = s.IsVerified,
                        Secret = s.Secret,
                    })
                        .Where(w => w.UserId == userId)
                        .FirstOrDefaultAsync(cancellationToken);
                    await _cacheService.SetAsync(CacheKeys.UserSetting_ + userId.ToString(), userSetting, TimeSpan.FromHours(4));
                }
            }
            SecuritySettingDto? data = await _cacheService.GetAsync<SecuritySettingDto>(CacheKeys.SecuritySettings);
            if (data == null)
            {
                data = await _applicationDbContext.SecuritySettings.Select(s => new SecuritySettingDto
                {
                    PasswordComplexity = new PasswordComplexityDto()
                    {
                        RequiredLength = s.RequiredLength,
                        RequireDigit = s.RequireDigit,
                        RequireLowercase = s.RequireLowercase,
                        RequireUppercase = s.RequireUppercase,
                        RequireNonAlphanumeric = s.RequireNonAlphanumeric,

                    },
                    RequireCaptchaOnLogin = s.RequireCaptchaOnLogin,
                    LogginAttemptCountLimit = s.LogginAttemptCountLimit,
                    LogginAttemptTimeSecondsLimit = s.LogginAttemptTimeSecondsLimit,
                    UserSetting = default!
                }).FirstOrDefaultAsync(cancellationToken);
                await _cacheService.SetAsync(CacheKeys.SecuritySettings, data, TimeSpan.FromHours(4));
            }


            data!.UserSetting = userSetting ?? new UserSettingDto();
            return data;
        }

        public async Task<bool> SetSettingsAsync(SecuritySettingDto input, CancellationToken cancellationToken)
        {
            var data = await _applicationDbContext.SecuritySettings
                                .OrderBy(s => s.Id)
                                .FirstOrDefaultAsync();
            int result;
            if (data == null)
            {
                await _applicationDbContext.SecuritySettings.AddAsync(new SecuritySetting()
                {
                    RequireDigit = input.PasswordComplexity.RequireDigit,
                    RequiredLength = input.PasswordComplexity.RequiredLength,
                    RequireLowercase = input.PasswordComplexity.RequireLowercase,
                    RequireNonAlphanumeric = input.PasswordComplexity.RequireUppercase,
                    RequireUppercase = input.PasswordComplexity.RequireUppercase,
                    RequireCaptchaOnLogin = input.RequireCaptchaOnLogin,
                    LogginAttemptCountLimit = input.LogginAttemptCountLimit,
                    LogginAttemptTimeSecondsLimit = input.LogginAttemptTimeSecondsLimit,
                });
            }
            else
            {
                data.RequireDigit = input.PasswordComplexity.RequireDigit;
                data.RequiredLength = input.PasswordComplexity.RequiredLength;
                data.RequireLowercase = input.PasswordComplexity.RequireLowercase;
                data.RequireNonAlphanumeric = input.PasswordComplexity.RequireNonAlphanumeric;
                data.RequireUppercase = input.PasswordComplexity.RequireUppercase;
                data.RequireCaptchaOnLogin = input.RequireCaptchaOnLogin;
                data.LogginAttemptCountLimit = input.LogginAttemptCountLimit;
                data.LogginAttemptTimeSecondsLimit = input.LogginAttemptTimeSecondsLimit;
                _applicationDbContext.SecuritySettings.Update(data);
            }
            result = await _applicationDbContext.SaveChangesAsync(cancellationToken);

            //clean cache
            await _cacheService.RemoveAsync(CacheKeys.SecuritySettings);
            await _cacheService.RemoveAsync(CacheKeys.UserSetting_ + _currentUser.UserId);

            return result > 0;

        }

    }
}
