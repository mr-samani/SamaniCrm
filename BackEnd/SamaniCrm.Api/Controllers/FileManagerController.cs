using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.FileManager.Commands;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Queries;
using SamaniCrm.Application.Localize.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers;

[Authorize]
public class FileManagerController : ApiBaseController
{

    private readonly IMediator _mediator;

    public FileManagerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("GetTreeFolders")]
    [Permission(AppPermissions.FileManager.List)]
    [ProducesResponseType(typeof(ApiResponse<List<FileNodeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTreeFolders()
    {
        List<FileNodeDto> result = await _mediator.Send(new GetTreeFoldersQuery());
        return ApiOk(result);
    }


    [HttpGet("GetFolderDetails")]
    [Permission(AppPermissions.FileManager.List)]
    [ProducesResponseType(typeof(ApiResponse<List<FileNodeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolderDetails(Guid parentId)
    {
        List<FileNodeDto> result = await _mediator.Send(new GetFolderDetailsQuery(parentId));
        return ApiOk(result);
    }


    [HttpPost("CreateFolder")]
    [Permission(AppPermissions.FileManager.CreateFolder)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateFolder(CreateFolderCommand request)
    {
        bool result = await _mediator.Send(request);
        return ApiOk(result);
    }


    [HttpPost("DeleteFileOrFolder")]
    [Permission(AppPermissions.FileManager.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFileOrFolder(DeleteFileOrFolderCommand request)
    {
        bool result = await _mediator.Send(request);
        return ApiOk(result);
    }




    [HttpGet("GetFileManagerIcons")]
    [Permission(AppPermissions.FileManager.List)]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFileManagerIcons()
    {
        List<string> result = await _mediator.Send(new GetFileManagerIconsQuery());
        return ApiOk(result);
    }


    [HttpPost("SetFolderIcon")]
    [Permission(AppPermissions.FileManager.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetFolderIcon(SetFolderIconCommand request)
    {
        bool result = await _mediator.Send(request);
        return ApiOk(result);
    }


    [HttpPost("Rename")]
    [Permission(AppPermissions.FileManager.Rename)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Rename(RenameCommand request)
    {
        bool result = await _mediator.Send(request);
        return ApiOk(result);
    }
}
