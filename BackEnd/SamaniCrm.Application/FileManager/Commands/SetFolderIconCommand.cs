using MediatR;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Commands;

public record SetFolderIconCommand:IRequest<bool>
{
    public Guid Id { get; set; }
    public required string Icon { get; set; }
}


public class SetFolderIconCommandHandler : IRequestHandler<SetFolderIconCommand, bool>
{
    private readonly IFileManagerService _fileManagerService;

    public SetFolderIconCommandHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<bool> Handle(SetFolderIconCommand request, CancellationToken cancellationToken)
    {
       return _fileManagerService.SetFolderIcon(request.Id, request.Icon,cancellationToken);
    }
}