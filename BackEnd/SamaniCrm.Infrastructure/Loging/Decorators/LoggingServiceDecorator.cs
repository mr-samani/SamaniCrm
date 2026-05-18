using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Logging.Interfaces;
using SamaniCrm.Core.Shared.Logging.Dtos;
using System.Diagnostics;
using System.Reflection;

namespace SamaniCrm.Infrastructure.Loging.Decorators;

/// <summary>
/// Decorator برای لاگ‌نویسی خودکار سرویس‌ها
/// </summary>
public class LoggingServiceDecorator<TInterface, TImplementation> : DispatchProxy
    where TImplementation : class, TInterface
{
    private TImplementation? _inner;
    private ILogService? _logService;

    public static TInterface Create(TImplementation inner,
                                     ILogService logService)
    {
        var proxy = DispatchProxy.Create<TInterface, LoggingServiceDecorator<TInterface, TImplementation>>();
        if (proxy is null)
            throw new InvalidOperationException("Failed to create proxy");

        // تنظیم فیلدهای private با reflection
        var proxyType = proxy.GetType();

        var innerField = proxyType.GetField("_inner", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var logServiceField = proxyType.GetField("_logService", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (innerField == null || logServiceField == null)
            throw new InvalidOperationException("Proxy fields not found");

        innerField.SetValue(proxy, inner);
        logServiceField.SetValue(proxy, logService);

        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (_inner == null || _logService == null)
            throw new InvalidOperationException("Proxy not initialized");

        var stopwatch = Stopwatch.StartNew();
        var methodName = targetMethod!.Name;
        var className = typeof(TImplementation).Name;

        var context = new LogContextDto
        {
            Source = $"{className}.{methodName}"
        };
        _logService.Log(LogLevel.Debug, "Calling {Class}.{Method} with args: {@Args}",
            null, context, className, methodName, args ?? []);

        try
        {
            var result = targetMethod.Invoke(_inner, args);
            stopwatch.Stop();

            // اگر متد async باشد
            if (result is Task task)
            {
                return HandleAsyncTask(task, stopwatch, className, methodName, args);
            }

            _logService.Log(LogLevel.Debug, "Completed {Class}.{Method} in {Duration}ms",
                null, context, className, methodName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logService.Log(LogLevel.Error, "Error in {Class}.{Method} after {Duration}ms",
                ex.InnerException ?? ex, context, className, methodName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    private async Task HandleAsyncTask(Task task, Stopwatch stopwatch,
                                        string className, string methodName, object?[]? args)
    {
        if (_inner == null || _logService == null)
            throw new InvalidOperationException("Proxy not initialized");

        var context = new LogContextDto
        {
            Source = $"{className}.{methodName}"
        };
        try
        {
            await task.ConfigureAwait(false);
            stopwatch.Stop();

            _logService.Log(LogLevel.Debug, "Completed async {Class}.{Method} in {Duration}ms",
                null, context, className, methodName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logService.Log(LogLevel.Error, "Error in async {Class}.{Method} after {Duration}ms",
                ex.InnerException ?? ex, context, className, methodName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }



}
