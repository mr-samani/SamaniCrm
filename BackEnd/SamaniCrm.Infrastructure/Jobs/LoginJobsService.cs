using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Console;


namespace SamaniCrm.Infrastructure.Jobs;



public class LoginJobsService : ILoginJobsService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISecuritySettingService _securitySettingService;

    public LoginJobsService(IApplicationDbContext dbContext, ISecuritySettingService securitySettingService)
    {
        _dbContext = dbContext;
        _securitySettingService = securitySettingService;
    }






    public Task ReleaseExpiredLocksAsync(PerformContext context)
    {
        return ReleaseExpiredLocksAsync(context, CancellationToken.None);
    }

    public async Task ReleaseExpiredLocksAsync(PerformContext context, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var securitySetting = await _securitySettingService.GetSettingsAsync(cancellationToken);
        var limitTryCount = securitySetting.LogginAttemptCountLimit;
        List<UserSetting> lockedUsers = await _dbContext.UserSetting
           .Where(u => u.AttemptCount >= limitTryCount &&
           u.LastAttemptAt.AddSeconds(securitySetting.LogginAttemptTimeSecondsLimit) <= now)
           .ToListAsync(cancellationToken);
        if (lockedUsers.Any())
        {
            foreach (var user in lockedUsers)
            {
                user.AttemptCount = 0;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        context.WriteLine($"🔓 {lockedUsers.Count} user(s) unlocked at {now:u}");
    }
}
