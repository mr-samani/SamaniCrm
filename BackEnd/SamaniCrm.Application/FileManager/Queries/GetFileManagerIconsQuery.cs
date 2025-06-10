using MediatR;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Queries;


public record GetFileManagerIconsQuery() : IRequest<List<string>>;



public class GetFileManagerIconsQueryHanlder : IRequestHandler<GetFileManagerIconsQuery, List<string>>
{
    private readonly IFileManagerService _fileManagerService;

    public GetFileManagerIconsQueryHanlder(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public async Task<List<string>> Handle(GetFileManagerIconsQuery request, CancellationToken cancellationToken)
    {
        return await _fileManagerService.GetFileManagerIcons(cancellationToken);
    }
}