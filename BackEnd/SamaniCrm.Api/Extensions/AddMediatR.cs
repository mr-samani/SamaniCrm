using MediatR;
using SamaniCrm.Application.Common.Behaviors;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Application.Queries.Role;
using SamaniCrm.Application.User.Queries;
using System.Reflection;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{


    /// <summary>
    /// Mediator Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // cfg.RegisterServicesFromAssembly(typeof(UserListQuery).Assembly);
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssembly(typeof(GetRoleQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(InitialAppQueryHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetCurrentUserQueryHandler).Assembly);


            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // TODO
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantValidationBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TenantAuthorizationBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));


        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
