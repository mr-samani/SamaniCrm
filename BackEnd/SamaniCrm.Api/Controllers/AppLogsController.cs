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

[Authorize(Roles = AppRoles.SysAdmin)]
public class AppLogsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AppLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }



    // ═══════════════════════════════════════════════════════════
    // تنظیمات
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// دریافت تنظیمات لاگ‌نویسی Tenant
    /// </summary>
    [HttpGet("GetSettings")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.TenantAppLogSetting.List)]
    [ProducesResponseType(typeof(ApiResponse<TenantAppLogSettingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings(Guid? tenantId)
    {
        var result = await _mediator.Send(new GetAppLogSettingQuery(tenantId));

        return ApiOk(result);
    }

    /// <summary>
    /// بروزرسانی تنظیمات لاگ‌نویسی
    /// </summary>
    [HttpPost("UpdateSettings")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.TenantAppLogSetting.Update)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSettings(UpdateAppLogSettingCommand input)
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
    [HttpPost("GetLogs")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AppLogEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogs(GetAppLogsQuery input)
    {
        var result = await _mediator.Send(input);
        return ApiOk(result);
    }

    /// <summary>
    /// جزئیات یک لاگ
    /// </summary>
    [HttpGet("GetLogDetail")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.Details)]
    [ProducesResponseType(typeof(ApiResponse<AppLogEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogDetail(Guid tenantId, long id)
    {
        var result = await _mediator.Send(new GetAppLogDetailsQuery(tenantId, id));
        return ApiOk(result);
    }

    /// <summary>
    /// آمار لاگ‌ها
    /// </summary>
    [HttpPost("GetStats")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.GetStats)]
    [ProducesResponseType(typeof(ApiResponse<AppLogStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(GetAppLogStatesQuery request)
    {
        var result = await _mediator.Send(request);
        return ApiOk(result);
    }


    /// <summary>
    /// حذف دستی لاگ‌های قدیمی
    /// </summary>
    [HttpPost("ManualCleanup")]
    [Permission(AppPermissions.LoggingSystem.AppLogs.ManualCleanUpLog)]
    [ProducesResponseType(typeof(ApiResponse<CleanupLogResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ManualCleanup(ManulaCleanupAppLogCommand input)
    {
        var result = await _mediator.Send(input);
        return ApiOk(result);
    }
}

