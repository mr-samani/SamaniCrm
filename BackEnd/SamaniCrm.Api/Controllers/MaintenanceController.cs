using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Maintenance.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
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
        [Permission(AppPermissions.CacheEntries_List)]
        [ProducesResponseType(typeof(ApiResponse<List<CacheEntryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCacheEntries([FromQuery] string? pattern)
        {
            List<CacheEntryDto> result = await _mediator.Send(new GetAllCacheEntriesQuery(pattern));
            return ApiOk<List<CacheEntryDto>>(result);
        }

        [HttpPost("DeleteCache")]
        [Permission(AppPermissions.CacheEntries_Delete)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCache(string key)
        {
            await _cache.RemoveAsync(key);
           // return NoContent();
           return ApiOk<string>(key);
        }

        [HttpPost("ClearAllCahces")]
        [Permission(AppPermissions.CacheEntries_ClearAll)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearAllCahces()
        {
            await _cache.ClearAsync();
          //  return NoContent();
          return ApiOk<string>(string.Empty);
        }




    }
}
