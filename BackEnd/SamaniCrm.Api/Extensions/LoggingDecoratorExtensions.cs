using Microsoft.Extensions.DependencyInjection;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Infrastructure.Loging.Decorators;
using SamaniCrm.Infrastructure.Services;
using Scrutor;


namespace SamaniCrm.Api.Extensions;

// ثبت در DI
public static class LoggingDecoratorExtensions
{
    public static IServiceCollection AddLoggedServices(this IServiceCollection services)
    {

        // اسکن تمام سرویس‌ها و اعمال Decorator
        services.Scan(scan => scan
            .FromAssemblyOf<IIdentityService>()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Controller")))
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            // .DecorateImplementedInterfaces()
            );



        return services;
    }
}
