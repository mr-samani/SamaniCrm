using SamaniCrm.Infrastructure.MappingProfile;

namespace SamaniCrm.Api.Extensions;


public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { },
        typeof(ExternalProviderProfile)
          //typeof(ProfileTypeFromAssembly1), 
          //typeof(ProfileTypeFromAssembly2) 
          );
        return services;
    }
}