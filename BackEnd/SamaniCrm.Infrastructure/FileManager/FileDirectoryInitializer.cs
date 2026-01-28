using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Core;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.FileManager
{
    public static class DefaultFolders
    {
        public static readonly List<FileFolder> BaseFolders =
                [ new FileFolder()
                   {
                       Name ="Publics",
                       IsFolder = true,
                       Icon ="Images/folder-icons/default-folder-network.svg",
                       IsStatic = true,
                       RelativePath = "Publics"
                   },
                   new FileFolder()
                   {
                       Name ="Documents",
                       IsFolder = true,
                       Icon ="Images/folder-icons/folder-documents.svg",
                       IsStatic = true,
                       RelativePath = "Documents"
                   },
                   new FileFolder()
                   {
                       Name ="Pictures",
                       IsFolder = true,
                       Icon ="Images/folder-icons/default-folder-pictures.svg",
                       IsStatic = true,
                       RelativePath = "Pictures"
                   },
                   new FileFolder()
                   {
                       Name ="Videos",
                       IsFolder = true,
                       Icon ="Images/folder-icons/folders-videos.svg",
                       IsStatic = true,
                       RelativePath = "Videos"
                   },
                   new FileFolder()
                   {
                       Name ="Music",
                       IsFolder = true,
                       Icon ="Images/folder-icons/default-folder-music.svg",
                       IsStatic = true,
                       RelativePath = "Music"
                   },
                   new FileFolder()
                   {
                       Name ="Favorites",
                       IsFolder = true,
                       Icon ="Images/folder-icons/favorites.svg",
                       IsStatic = true,
                       RelativePath = "Favorites"
                   },
            //Page builder plugins
                   new FileFolder()
                   {
                       Id = AppConsts.PluginsFolderId,
                       Name ="Plugins",
                       IsFolder = true,
                       Icon ="Images/folder-icons/plugins.svg",
                       IsStatic = true,
                       RelativePath = "Plugins"
                   }
                ];
    }

    public class FileDirectoryInitializer
    {
        private readonly IWebHostEnvironment _env;
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FileDirectoryInitializer(IWebHostEnvironment env, IApplicationDbContext dbContext, IConfiguration configuration)
        {
            _env = env;
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public async Task EnsureBaseDirectoriesAsync()
        {
            // var wwwrootPath = Path.Combine(_env.WebRootPath, "uploads");
            var settings = _configuration.GetSection("FileManager").Get<FileManagerSetting>() ?? new FileManagerSetting();
            var rootPath = settings.PublicFolderPath;

            if (Directory.Exists(rootPath) == false)
            {
                Directory.CreateDirectory(rootPath);
            }

            foreach (var folder in DefaultFolders.BaseFolders)
            {
                var fullPath = Path.Combine(rootPath, folder.Name);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var existsInDb = await _dbContext.FileFolders.AnyAsync(f => f.Name == folder.Name && f.ParentId == null);
                if (!existsInDb)
                {
                    _dbContext.FileFolders.Add(folder);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
