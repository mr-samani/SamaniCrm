using SamaniCrm.Core.Shared.Settings;
using SamaniCrm.Infrastructure.FileManager;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<FileManagerSettings>(configuration.GetSection("FileManager"));
        service.Configure<CaptchaSettings>(configuration.GetSection("Captcha"));
        return service;
    }

}
