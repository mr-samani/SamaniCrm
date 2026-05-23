using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Infrastructure.BackgroundServices;
using SamaniCrm.Infrastructure.Loging;
using SamaniCrm.Infrastructure.Loging.Sinks;
using Scalar.AspNetCore;
using SamaniCrm.Infrastructure.Loging.Filters;



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
        services.AddScoped<ILogConfigurationService, LogConfigurationService>();
        services.AddScoped<ILogService, LogService>();
        // services.AddScoped<ILogRetentionService, LogRetentionService>();


        // Background Service
        services.AddHostedService<LogRetentionService>();

        // Action Filter
        services.AddLoggingInterceptors();

        // Decorator برای Service ها
        services.AddLoggedServices();

        return services;
    }




}

