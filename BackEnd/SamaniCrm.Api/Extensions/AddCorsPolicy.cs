namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{



    /// <summary>
    /// اجازه دسترسی به برنامه توسط کلاینت ها
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCors", policy =>
            {
                policy.WithOrigins("http://localhost:5753", "https://localhost:5753", "https://localhost:5754")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithExposedHeaders("Location", "Upload-Offset", "Tus-Resumable", "Upload-Length", "Fileid");
            });
        });

        return services;
    }
}