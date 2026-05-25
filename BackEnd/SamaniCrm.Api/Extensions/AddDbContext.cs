using Microsoft.EntityFrameworkCore;
using SamaniCrm.Infrastructure;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        // ✅ DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(30);
            }),
            ServiceLifetime.Scoped);


        return services;
    }
}