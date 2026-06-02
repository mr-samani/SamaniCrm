using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Logging.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Logging.Dtos;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    [Authorize]
    public class SecurityLogController : ApiBaseController
    {

        private readonly IMediator _mediator;

        public SecurityLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ═══════════════════════════════════════════════════════════
        // مشاهده لاگ‌ها
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// لیست لاگ‌ها با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpPost("GetLogs")]
        [Permission(AppPermissions.LoggingSystem.SecurityLogs.List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<SecurityLogDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLogs(GetSecurityLogsQuery input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


        [HttpGet("GetLastLoginList")]
        [Permission(AppPermissions.LoggingSystem.SecurityLogs.LastLoginInfo)]
        [ProducesResponseType(typeof(ApiResponse<List<LastLoginDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLastLoginList()
        {
            var result = await _mediator.Send(new GetLastLoginInfoQuery());
            return ApiOk(result);
        }
    }
}
