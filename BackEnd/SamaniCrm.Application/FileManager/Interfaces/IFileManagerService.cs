using SamaniCrm.Application.FileManager.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Interfaces;

public interface IFileManagerService
{
    Task<List<FileNodeDto>> GetFolderTreeAsync(string? rootPath = null);

    Task<bool> CreateFolder(string name, bool isPublic, Guid? parentId, CancellationToken cancellationToken);

    Task<List<FileNodeDto>> GetFolderDetails(Guid Id, CancellationToken cancellationToken);

    Task<List<string>> GetFileManagerIcons(CancellationToken cancellationToken);
    Task<bool> DeleteFileOrFolder(Guid Id, CancellationToken cancellationToken);

    Task<bool> SetFolderIcon(Guid Id, string Icon, CancellationToken cancellationToken);

    Task<Guid> UploadFile(Guid ParentId, Stream fileStream, string fileName, CancellationToken cancellationToken);

    Task<FileNodeDto?> GetFileInfo(Guid Id, CancellationToken cancellationToken);    

}
