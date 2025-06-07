using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.FileManager
{
    public class LocaleFileManagerService : IFileManagerService
    {
        private readonly IConfiguration _configuration;
        private FileManagerSetting settings;
        private string publicRootPath;
        public LocaleFileManagerService(IConfiguration configuration)
        {
            settings = configuration.GetSection("FileManager").Get<FileManagerSetting>() ?? new FileManagerSetting();
            publicRootPath = settings.PublicFolderPath;
            _configuration = configuration;
        }

        public async Task<List<FileNodeDto>> GetFolderTreeAsync(string? rootPath)
        {
            if (rootPath == null)
                rootPath = publicRootPath;

            var result = new List<FileNodeDto>();

            foreach (var dir in Directory.GetDirectories(rootPath))
            {
                var node = new FileNodeDto
                {
                    Name = Path.GetFileName(dir),
                    FullPath = dir,
                    IsFolder = true,
                    Children = await GetFolderTreeAsync(dir)
                };
                result.Add(node);
            }

            foreach (var file in Directory.GetFiles(rootPath))
            {
                result.Add(new FileNodeDto
                {
                    Name = Path.GetFileName(file),
                    FullPath = file,
                    IsFolder = false
                });
            }

            return result;

        }




        public async Task<bool> CreateFolder(string name, Guid? parentId)
        {
            var path = Path.Combine(publicRootPath, name);
            var result = Directory.CreateDirectory(path);
            return result != null;
        }


    }
}
