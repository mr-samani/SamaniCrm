namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{


    /// <summary>
    /// Swagger Configuration
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", opt =>
        {
            opt.AddSchemaTransformer<SchemaTransformer>();
            opt.AddOperationTransformer<OperationSchmaTransformer>();

        });
        return services;
    }
}