using MediatR;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Commands;

public record CreateFolderCommand(string Name, bool IsPublic, Guid? ParentId) : IRequest<bool>;


public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, bool>
{
    private readonly IFileManagerService _fileManagerService;

    public CreateFolderCommandHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<bool> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        return _fileManagerService.CreateFolder(request.Name, request.IsPublic, request.ParentId, cancellationToken);
    }
}