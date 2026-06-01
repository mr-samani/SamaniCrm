using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Infrastructure.BackgroundServices;
using Scalar.AspNetCore;
using SamaniCrm.Infrastructure.Loging.AppLogs;
using SamaniCrm.Infrastructure.Loging.AppLogs.Sinks;
using SamaniCrm.Infrastructure.Loging.AppLogs.Filters;



namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{    


    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration config)
    {
        // Sink ها
        services.AddSingleton<FileLogSink>();
        services.AddScoped<DatabaseLogSink>();
        services.AddSingleton<TelegramLogSink>();
        services.AddSingleton<BaleLogSink>();
        services.AddSingleton<ExternalApiLogSink>();

        // ثبت Sink ها به صورت IEnumerable
        services.AddScoped<IEnumerable<ILogSink>>(sp => new ILogSink[]
        {
            sp.GetRequiredService<FileLogSink>(),
            sp.GetRequiredService<DatabaseLogSink>(),
            sp.GetRequiredService<TelegramLogSink>(),
            sp.GetRequiredService<BaleLogSink>(),
            sp.GetRequiredService<ExternalApiLogSink>()
        });

        // Core Services
        services.AddScoped<IAppLogConfigurationService, AppLogConfigurationService>();
        services.AddScoped<IAppLogService, AppLogService>();
        // services.AddScoped<ILogRetentionService, LogRetentionService>();


        // Background Service
        services.AddHostedService<LogRetentionService>();

        // Action Filter
        services.AddLoggingInterceptors();

        // Decorator برای Service ها
        services.AddAppLoggedServices();

        return services;
    }




}

