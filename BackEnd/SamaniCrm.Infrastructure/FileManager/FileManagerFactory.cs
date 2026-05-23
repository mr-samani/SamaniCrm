using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.FileManager.Interfaces;

namespace SamaniCrm.Infrastructure.FileManager;

public static class FileManagerFactory
{
    public static IServiceCollection AddFileManagerService(this IServiceCollection services, IConfiguration configuration)
    {

        var settings = configuration.GetSection("FileManager")
            .Get<FileManagerSettings>() ?? new FileManagerSettings();
        var rootPath = settings.PublicFolderPath;

        if (rootPath == null)
        {
            throw new NotFoundException("Root path not specified");
        }

        if (Directory.Exists(rootPath) == false)
        {
            Directory.CreateDirectory(rootPath);
        }

        switch (settings.Provider)
        {
            case "locale":
                services.AddScoped<IFileManagerService, LocaleFileManagerService>();
                break;
        }
        return services;

    }
}
