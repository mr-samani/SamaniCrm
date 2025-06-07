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

    Task<bool> CreateFolder(string name, Guid? parentId);
}
