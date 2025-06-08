using MediatR;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Commands;

public record DeleteFileOrFolderCommand :IRequest<bool>
{
    public Guid Id { get; set; }
}



public class DeleteFileOrFolderCommandHandler : IRequestHandler<DeleteFileOrFolderCommand, bool>
{
    private readonly IFileManagerService _fileManagerService;

    public DeleteFileOrFolderCommandHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<bool> Handle(DeleteFileOrFolderCommand request, CancellationToken cancellationToken)
    {
       return _fileManagerService.DeleteFileOrFolder(request.Id, cancellationToken);
    }
}