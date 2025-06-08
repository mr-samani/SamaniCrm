using MediatR;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Queries;

public record GetFolderDetailsQuery(Guid ParentId) : IRequest<List<FileNodeDto>>;



public class GetFolderDetailsQueryHanlder : IRequestHandler<GetFolderDetailsQuery, List<FileNodeDto>>
{
    private readonly IFileManagerService _fileManagerService;

    public GetFolderDetailsQueryHanlder(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public async Task<List<FileNodeDto>> Handle(GetFolderDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _fileManagerService.GetFolderDetails(request.ParentId, cancellationToken);
    }
}