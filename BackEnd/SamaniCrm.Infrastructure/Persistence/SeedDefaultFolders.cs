using Microsoft.EntityFrameworkCore;
using SamaniCrm.Core;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.FileManager;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace SamaniCrm.Infrastructure.Persistence;

public static class SeedDefaultFolders
{
    public static async Task TrySeedAsync(ApplicationDbContext dbContext, FileManagerSettings settings)
    {
        Console.WriteLine("Seeding default folders...");

        // var wwwrootPath = Path.Combine(_env.WebRootPath, "uploads");
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

            var existsInDb = await dbContext.FileFolders.AnyAsync(f => f.Name == folder.Name && f.ParentId == null);
            if (!existsInDb)
            {
                dbContext.FileFolders.Add(folder);
            }
        }

        await dbContext.SaveChangesAsync();

        Console.WriteLine("seed folders ended");
    }
}


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
                       Icon ="Images/folder-icons/folder-dropbox.svg",
                       IsStatic = true,
                       RelativePath = "Plugins"
                   }
            ];
}

