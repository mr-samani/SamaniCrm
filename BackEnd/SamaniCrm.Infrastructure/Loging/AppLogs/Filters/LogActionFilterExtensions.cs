using Microsoft.Extensions.DependencyInjection;

namespace SamaniCrm.Infrastructure.Loging.AppLogs.Filters;

// ثبت در DI
public static class LogActionFilterExtensions
{
    public static IServiceCollection AddLoggingInterceptors(this IServiceCollection services)
    {
        services.AddScoped<LogActionFilter>();
        services.AddMvc(options =>
        {
            options.Filters.Add<LogActionFilter>();
        });
        return services;
    }
}