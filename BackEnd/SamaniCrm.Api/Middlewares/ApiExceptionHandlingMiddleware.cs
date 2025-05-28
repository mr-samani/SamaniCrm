using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Host.Models;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace SamaniCrm.Host.Middlewares;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ApiExceptionHandlingMiddleware(RequestDelegate next,
                                          ILogger<ApiExceptionHandlingMiddleware> logger,
                                          IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (CustomValidationException vex)
        {
            await HandleCustomValidationExceptionAsync(context, vex);
        }
        catch (BaseAppException appEx)
        {
            await HandleKnownExceptionAsync(context, appEx.StatusCode, appEx.Message);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        var errors = ex.Errors.Select(e => new ApiError
        {
            Field = e.PropertyName,
            Message = e.ErrorMessage
        }).ToList();

        await WriteResponseAsync(context, HttpStatusCode.BadRequest, ApiResponse<object>.Fail(errors));
    }

    private async Task HandleCustomValidationExceptionAsync(HttpContext context, CustomValidationException vex)
    {
        var errors = vex.Errors.Select(e => new ApiError
        {
            Field = e.Key,
            Message = e.Value.FirstOrDefault() ?? ""
        }).ToList();

        await WriteResponseAsync(context, HttpStatusCode.BadRequest, ApiResponse<object>.Fail(errors));
    }

    private async Task HandleKnownExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        var error = new ApiError { Message = message };
        await WriteResponseAsync(context, statusCode, ApiResponse<object>.Fail(new List<ApiError> { error }));
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception");

        var error = new ApiError
        {
            Message = "An unexpected error occurred.",
            Detail = _env.IsDevelopment() ? ex.ToString() : null
        };

        await WriteResponseAsync(context, HttpStatusCode.InternalServerError, ApiResponse<object>.Fail(new List<ApiError> { error }));
    }

    private async Task WriteResponseAsync<T>(HttpContext context, HttpStatusCode statusCode, ApiResponse<T> response)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
