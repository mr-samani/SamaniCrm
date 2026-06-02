using SamaniCrm.Application.Common.Interfaces;


namespace SamaniCrm.Api.Extensions;

// ثبت در DI
public static class AddAppLoggingDecorator
{
    public static IServiceCollection AddAppLoggedServices(this IServiceCollection services)
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
