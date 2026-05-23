namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddHelthChecks(this IServiceCollection services, IConfiguration config)
    {
        // TODO
        //services.AddHealthChecks()
        // .AddDbContextCheck<ApplicationDbContext>("master_db");
        //  .AddCheck<TenantDatabaseHealthCheck>("tenant_db");
        return services;
    }
}