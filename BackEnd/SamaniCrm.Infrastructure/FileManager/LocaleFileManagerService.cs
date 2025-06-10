using Duende.IdentityModel;
using HeyRed.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeDetective;
using MimeDetective.Engine;
using Pipelines.Sockets.Unofficial.Arenas;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using SamaniCrm.Infrastructure.FileManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MimeDetective.Definitions.DefaultDefinitions;



namespace SamaniCrm.Infrastructure.FileManager
{
    public class LocaleFileManagerService : IFileManagerService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ImageResizeService _imageResizeService = new ImageResizeService();

        private FileManagerSetting settings;
        private string publicRootPath;
        public LocaleFileManagerService(IConfiguration configuration, IApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            settings = configuration.GetSection("FileManager").Get<FileManagerSetting>() ?? new FileManagerSetting();
            publicRootPath = settings.PublicFolderPath;
            _configuration = configuration;
            _dbContext = dbContext;
            _env = env;
        }

        public async Task<List<FileNodeDto>> GetFolderTreeAsync(string? rootPath = null)
        {
            var all = await _dbContext.FileFolders
                            .Where(x => x.IsFolder == true)
                            .Include(m => m.Children.Where(x => x.IsFolder == true))
                            .ToListAsync();
            var roots = all.Where(m => m.ParentId == null).ToList();
            var result = roots.Select(m => MapToDtoRecursive(m)).ToList();
            return result ?? [];
        }
        private static FileNodeDto MapToDtoRecursive(FileFolder item)
        {
            return new FileNodeDto
            {
                Id = item.Id,
                Name = item.Name,
                ByteSize = item.ByteSize,
                ContentType = item.ContentType,
                Extension = item.Extension,
                Icon = item.Icon,
                IsFolder = item.IsFolder,
                IsPublic = item.IsPublic,
                IsStatic = item.IsStatic,
                ParentId = item.ParentId,
                RelativePath = item.RelativePath,
                Thumbnails = item.Thumbnails,
                Children = item.Children?
                    .Select(c => MapToDtoRecursive(c))
                    .ToList() ?? [],
                CreationTime = item.CreationTime.ToUniversalTime(),
            };
        }



        private async Task<List<FileNodeDto>> GetFolderTreeFromDiscAsync(string? rootPath)
        {
            if (rootPath == null)
                rootPath = publicRootPath;

            var result = new List<FileNodeDto>();

            foreach (var dir in Directory.GetDirectories(rootPath))
            {
                var node = new FileNodeDto
                {
                    Name = Path.GetFileName(dir),
                    RelativePath = dir,
                    IsFolder = true,
                    Children = await GetFolderTreeFromDiscAsync(dir)
                };
                result.Add(node);
            }

            foreach (var file in Directory.GetFiles(rootPath))
            {
                result.Add(new FileNodeDto
                {
                    Name = Path.GetFileName(file),
                    RelativePath = file,
                    IsFolder = false
                });
            }

            return result;

        }




        public async Task<bool> CreateFolder(string name, bool isPublic, Guid? parentId, CancellationToken cancellationToken)
        {


            var parent = await _dbContext.FileFolders.Where(x => x.Id == parentId).FirstOrDefaultAsync(cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException("Parent not found");
            }
            if (parent.RelativePath.StartsWith("/"))
            {
                parent.RelativePath = parent.RelativePath.Substring(1);
            }
            var path = Path.Combine(publicRootPath, parent.RelativePath, name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var relativePath = path.Replace(publicRootPath, "").Replace("\\", "/");
            if (relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }
            var folder = new FileFolder()
            {
                Name = name,
                IsFolder = true,
                IsPublic = isPublic,
                IsStatic = false,
                ContentType = "Directory",
                Extension = "",
                RelativePath = relativePath,
            };
            if (parentId != null)
            {
                folder.ParentId = parentId.Value;
            }

            _dbContext.FileFolders.Add(folder);
            var result = _dbContext.SaveChanges();

            return result > 0;
        }



        public async Task<List<FileNodeDto>> GetFolderDetails(Guid ParentId, CancellationToken cancellationToken)
        {
            var parent = await _dbContext.FileFolders.FindAsync(ParentId, cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException("Parent not found");
            }
            var result = await _dbContext.FileFolders.Where(x => x.ParentId == ParentId)
                .Select(item => new FileNodeDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    ByteSize = item.ByteSize,
                    ContentType = item.ContentType,
                    Extension = item.Extension,
                    Icon = item.Icon,
                    IsFolder = item.IsFolder,
                    IsPublic = item.IsPublic,
                    IsStatic = item.IsStatic,
                    ParentId = item.ParentId,
                    RelativePath = item.RelativePath,
                    Thumbnails = item.Thumbnails,
                    CreationTime = item.CreationTime.ToUniversalTime(),
                })
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<bool> DeleteFileOrFolder(Guid Id, CancellationToken cancellationToken)
        {
            var found = await _dbContext.FileFolders.FindAsync(Id, cancellationToken);
            if (found == null)
            {
                throw new NotFoundException("Parent not found");
            }

            found.IsDeleted = true;
            found.DeletedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            // چون softDelete می باشد - اصل فایل از روی دیسک حذف نمی شود
            return result > 0;
        }

        public async Task<List<string>> GetFileManagerIcons(CancellationToken cancellationToken)
        {
            var wwwrootPath = Path.Combine(_env.WebRootPath);
            var path = Path.Combine(wwwrootPath, "Images", "folder-icons");
            var files = Directory.GetFiles(path);
            var relativePaths = new List<string>();
            foreach (var file in files)
            {
                // var relativePath = Path.GetRelativePath("./", file);
                var relativePath = file.Replace(wwwrootPath, string.Empty);
                relativePaths.Add(relativePath);
            }

            return relativePaths.ToList();
        }

        public async Task<bool> SetFolderIcon(Guid Id, string Icon, CancellationToken cancellationToken)
        {
            var found = await _dbContext.FileFolders.FindAsync(Id, cancellationToken);
            if (found == null)
            {
                throw new NotFoundException("Parent not found");
            }

            found.Icon = Icon;
            found.LastModifiedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<Guid> UploadFile(Guid parentId, Stream fileStream, string fileName, CancellationToken cancellationToken)
        {
            // بررسی وجود پوشه پدر
            var parent = await _dbContext.FileFolders
                .FirstOrDefaultAsync(x => x.Id == parentId && x.IsFolder, cancellationToken);

            if (parent == null)
                throw new NotFoundException("Parent folder not found");
            if (parent.RelativePath.StartsWith("/"))
            {
                parent.RelativePath = parent.RelativePath.Substring(1);
            }
            // مسیر نهایی ذخیره‌سازی
            var destinationFolder = Path.Combine(publicRootPath, parent.RelativePath);

            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            // خواندن فایل در حافظه برای تشخیص MIME
            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms, cancellationToken);
            var fileBytes = ms.ToArray();

            // تشخیص MIME type
            var inspector = new ContentInspectorBuilder
            {
                Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
            }.Build();

            var result = inspector.Inspect(fileBytes);

            if (result == null || result.Count() == 0)
                throw new UserFriendlyException("Could not determine file type");

            var mime = result.First().Definition.File.MimeType;           // مانند image/png
            var extension = result.First().Definition.File.Extensions.First(); // مانند png

            // بررسی مجاز بودن نوع فایل
            var allowedMimeTypes = new[] { "image/png", "image/jpeg", "image/tiff" };
            if (!allowedMimeTypes.Contains(mime))
                throw new UserFriendlyException($"File type {mime} is not allowed");

            // تولید نام نهایی فایل
            var fileId = Guid.NewGuid();
            var finalFileName = $"{fileId}.{extension}";
            var finalFilePath = Path.Combine(destinationFolder, finalFileName);

            await File.WriteAllBytesAsync(finalFilePath, fileBytes, cancellationToken);
            var relativePath = finalFilePath.Replace(publicRootPath, "").Replace("\\", "/");
            if (relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }
            var thumbnails = await _imageResizeService.GenerateThumbnailsAsync(fileId, fileStream, extension, destinationFolder, cancellationToken);
            var entity = new FileFolder
            {
                Id = fileId,
                ByteSize = fileBytes.Length,
                ContentType = mime,
                Extension = extension,
                IsFolder = false,
                IsStatic = false,
                Name = fileName,
                IsPublic = parent.IsPublic,
                RelativePath = relativePath,
                ParentId = parent.Id,
                Icon = FileIcon.GetIcon(extension),
                Thumbnails = thumbnails != null ? thumbnails.ToString() : null
            };

            await _dbContext.FileFolders.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return fileId;
        }

        public async Task<FileNodeDto?> GetFileInfo(Guid Id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.FileFolders
                .AsNoTracking()
                .Where(x => x.Id == Id && !x.IsFolder)
                .Select(s => new FileNodeDto
                {
                    Id = s.Id,
                    ByteSize = s.ByteSize,
                    ContentType = s.ContentType,
                    Extension = s.Extension,
                    IsFolder = s.IsFolder,
                    IsStatic = s.IsStatic,
                    Name = s.Name,
                    IsPublic = s.IsPublic,
                    RelativePath = s.RelativePath,
                    ParentId = s.ParentId,
                    Icon = s.Icon,
                    Thumbnails = s.Thumbnails
                })
                .FirstOrDefaultAsync(cancellationToken);
            return entity;
        }
    }
}
