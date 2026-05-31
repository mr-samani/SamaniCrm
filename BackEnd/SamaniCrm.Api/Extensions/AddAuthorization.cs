using SamaniCrm.Core.Shared.Consts;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{


    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        //services.AddAuthorization(options =>
        //{
        //    foreach (var permission in Enum.GetNames(typeof(Permission)))
        //    {
        //        options.AddPolicy($"Permission:{permission}", policy =>
        //        {
        //            policy.Requirements.Add(new PermissionRequirement(permission));
        //        });
        //    }
        //});

        //services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
            "CanImpersonate",
            policy =>
            {
                policy.RequireRole(AppRoles.SysAdmin);
            });

            options.AddPolicy("TenantOwner", policy =>
                policy.RequireClaim("tenant_role", "Owner"));
            options.AddPolicy("TenantAdmin", policy =>
                policy.RequireClaim("tenant_role", "Owner", "Admin"));
        });

        return services;
    }
}


