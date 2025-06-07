using MediatR;
using Microsoft.Extensions.Configuration;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Queries;

public record GetTreeFoldersQuery : IRequest<List<FileNodeDto>>;


public class GetTreeFoldersQueryHandler : IRequestHandler<GetTreeFoldersQuery, List<FileNodeDto>>
{

    private readonly IFileManagerService _FileManagerService;

    public GetTreeFoldersQueryHandler(IFileManagerService fileManagerService)
    {
        _FileManagerService = fileManagerService;
    }

    public Task<List<FileNodeDto>> Handle(GetTreeFoldersQuery request, CancellationToken cancellationToken)
    {
        var result = _FileManagerService.GetFolderTreeAsync();
        return result;
    }





}
