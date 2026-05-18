using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public class HangfireJobStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                jobManager.AddOrUpdate<ILoginJobsService>(
                    recurringJobId: "ReleaseExpiredLocksJob",
                    methodCall: job => job.ReleaseExpiredLocksAsync(null!),
                    cronExpression: "*/30 * * * * *",
                     options: new RecurringJobOptions()
                     {
                         MisfireHandling = MisfireHandlingMode.Relaxed,
                         TimeZone = TimeZoneInfo.Utc,
                     }
                );

                next(app);
            };
        }
    }

}





