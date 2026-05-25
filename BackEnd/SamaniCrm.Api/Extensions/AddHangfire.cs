using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseColouredConsoleLogProvider()
                .UseConsole()
                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        // Add the processing server as IHostedService
        services.AddHangfireServer();


        // AddHangfireJobs
        services.AddSingleton<IStartupFilter, HangfireJobStartupFilter>();



        return services;
    }

     
}