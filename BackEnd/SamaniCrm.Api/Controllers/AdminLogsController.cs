using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Logging.Commands;
using SamaniCrm.Application.Features.Logging.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Logging;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers;

[Route("api/admin/logs")]
[Authorize(Roles = AppRoles.Administrator)]
public class AdminLogController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AdminLogController(IMediator mediator)
    {
        _mediator = mediator;
    }

  

    // ═══════════════════════════════════════════════════════════
    // تنظیمات
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// دریافت تنظیمات لاگ‌نویسی Tenant
    /// </summary>
    [HttpGet("settings/{tenantId:guid}")]
    [Permission(AppPermissions.LoggingSystem.TenantLogSetting.List)]
    [ProducesResponseType(typeof(ApiResponse<TenantLogSettingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings(GetLogSettingQuery input)
    {
        var result = await _mediator.Send(input);

        return ApiOk(result);
    }

    /// <summary>
    /// بروزرسانی تنظیمات لاگ‌نویسی
    /// </summary>
    [HttpPut("settings/{tenantId:guid}")]
    [Permission(AppPermissions.LoggingSystem.TenantLogSetting.Update)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSettings(UpdateLogSettingCommand input)
    {
        var result = await _mediator.Send(input);
        return ApiOk(result);
    }

    // ═══════════════════════════════════════════════════════════
    // مشاهده لاگ‌ها
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// لیست لاگ‌ها با فیلتر و صفحه‌بندی
    /// </summary>
    [HttpGet("entries/{tenantId:guid}")]
    [Permission(AppPermissions.LoggingSystem.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<LogEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs(        GetLogsQuery input)
    {
       var result= await _mediator.Send(input);
        return ApiOk(result);
    }

    /// <summary>
    /// جزئیات یک لاگ
    /// </summary>
    [HttpGet("entries/{tenantId:guid}/{id:long}")]
    [Permission(AppPermissions.LoggingSystem.Details)]
    [ProducesResponseType(typeof(ApiResponse<LogEntryDto>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetLogDetail(Guid tenantId, long id)
    {
         var result= await _mediator.Send(new GetLogDetailsQuery(tenantId,id));
        return ApiOk(result);
    }

    /// <summary>
    /// آمار لاگ‌ها
    /// </summary>
    [HttpGet("getStats")]
    [Permission(AppPermissions.LoggingSystem.GetStats)]
    [ProducesResponseType(typeof(ApiResponse<LogStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(GetLogStatesQuery request)
    {
        var result =await _mediator.Send(request);
        return ApiOk(result);
    } 


    /// <summary>
    /// حذف دستی لاگ‌های قدیمی
    /// </summary>
    [HttpDelete("cleanup/{tenantId:guid}")]
    [Permission(AppPermissions.LoggingSystem.ManualCleanUpLog)]
    [ProducesResponseType(typeof(ApiResponse<CleanupLogResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ManualCleanup(ManulaCleanupLogCommand input)
    {
        var result= await _mediator.Send(input);
        return ApiOk(result);
    }
}

