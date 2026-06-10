using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Maintenance.Commands;
using SamaniCrm.Application.Maintenance.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers;

[Authorize]
public class MaintenanceController : ApiBaseController
{


    private readonly IMediator _mediator;
    private readonly ICacheService _cache;

    public MaintenanceController(IMediator mediator, ICacheService cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpGet("GetAllCacheEntries")]
    [Permission(AppPermissions.Maintenance.Cache.List)]
    [ProducesResponseType(typeof(ApiResponse<List<CacheEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCacheEntries([FromQuery] string? pattern)
    {
        List<CacheEntryDto> result = await _mediator.Send(new GetAllCacheEntriesQuery(pattern));
        return ApiOk<List<CacheEntryDto>>(result);
    }

    [HttpPost("DeleteCache")]
    [Permission(AppPermissions.Maintenance.Cache.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCache(string key)
    {
        var result = await _mediator.Send(new RemoveCacheCommand(key));
        //  return NoContent();
        return ApiOk(result);
    }

    [HttpPost("ClearAllCahces")]
    [Permission(AppPermissions.Maintenance.Cache.ClearAll)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearAllCahces()
    {
        var result = await _mediator.Send(new ClearAllCachesCommand());
        //  return NoContent();
        return ApiOk(result);
    }



    [HttpPost("GetCacheInfo")]
    [Permission(AppPermissions.Maintenance.Cache.ViewData)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCacheInfo(string key)
    {
        var result = await _mediator.Send(new GetCacheInfoQuery(key));
        //  return NoContent();
        return ApiOk(result);
    }


}
