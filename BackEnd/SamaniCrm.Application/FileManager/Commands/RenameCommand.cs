using MediatR;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Commands;

public record RenameCommand:IRequest<bool>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}


public class RenameCommandHandler : IRequestHandler<RenameCommand, bool>
{
    private readonly IFileManagerService _fileManagerService;

    public RenameCommandHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<bool> Handle(RenameCommand request, CancellationToken cancellationToken)
    {
       return _fileManagerService.Rename(request.Id, request.Name,cancellationToken);
    }
}