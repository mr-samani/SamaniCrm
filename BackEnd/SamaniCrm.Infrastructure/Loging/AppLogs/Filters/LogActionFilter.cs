using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;


namespace SamaniCrm.Infrastructure.Loging.AppLogs.Filters;

//این کلاس قبل و بعد از اجرای هر Action در Controller اجرا می‌شود.
// هر بار که یک API یا اکشن صدا زده شود، این فیلتر وارد عمل می‌شود.

/// <summary>
///  Action Filter لاگینگ سراسری در ASP.NET Core :
/// ✅ شروع اجرای هر اکشن را لاگ می‌کند
/// 
/// ✅ زمان اجرای آن را اندازه می‌گیرد
/// 
/// ✅ اگر خطا یا HTTP status خطا(>=400) رخ بدهد آن را ثبت می‌کند
/// 
/// ✅ در پایان نتیجه موفق یا ناموفق بودن را لاگ می‌کند
/// </summary>
public class LogActionFilter : IAsyncActionFilter
{
    private readonly IAppLogService _logService;
    private readonly IApplicationDbContext _dbContext;

    public LogActionFilter(IAppLogService logService, IApplicationDbContext dbContext)
    {
        _logService = logService;
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                              ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.HttpContext.TraceIdentifier;

        // لاگ شروع
        var logContext = BuildContext(context, correlationId);

        _logService.Log(LogLevel.Information,
            "Action started: {Controller}.{Action}",
            null,
            logContext,
            context.Controller.GetType().Name,
            context.ActionDescriptor.RouteValues["action"] ?? context.ActionDescriptor.DisplayName ?? "");

        Exception? caughtException = null;
        ObjectResult? result = null;

        try
        {
            var executedContext = await next();
            stopwatch.Stop();

            result = executedContext.Result as ObjectResult;

            if (executedContext.Exception != null)
            {
                caughtException = executedContext.Exception;
            }
            else if (result?.StatusCode >= 400)
            {
                caughtException = new HttpRequestException(
                    "HTTP Error",
                    null,
                    (HttpStatusCode)(result.StatusCode ?? 500)
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            caughtException = ex;
        }

        // لاگ پایان
        if (caughtException != null)
        {
            _logService.Log(LogLevel.Error,
                "Action failed: {Controller}.{Action} - {Duration}ms",
                caughtException, logContext,
                context.Controller.GetType().Name,
                context.ActionDescriptor.RouteValues["action"] ?? context.ActionDescriptor.DisplayName ?? "",
                stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logService.Log(LogLevel.Information,
                "Action completed: {Controller}.{Action} - {Duration}ms - Status: {StatusCode}",
               null, logContext,
                context.Controller.GetType().Name,
                context.ActionDescriptor.RouteValues["action"] ?? context.ActionDescriptor.DisplayName ?? "",
                stopwatch.ElapsedMilliseconds,
                result?.StatusCode ?? 200);
        }
    }

    private AppLogContextDto BuildContext(ActionExecutingContext context, string correlationId)
    {
        return new AppLogContextDto
        {
            IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            CorrelationId = correlationId,
            ControllerName = context.Controller.GetType().Name,
            ActionName = context.ActionDescriptor.RouteValues["action"] ?? context.ActionDescriptor.DisplayName,
            HttpMethod = context.HttpContext.Request.Method,
            RequestPath = context.HttpContext.Request.Path,
            ExtraData = new Dictionary<string, object>
            {
                ["ActionArguments"] = context.ActionArguments
                    .ToDictionary(k => k.Key, v => v.Value ?? "null")
            }
        };
    }
}
