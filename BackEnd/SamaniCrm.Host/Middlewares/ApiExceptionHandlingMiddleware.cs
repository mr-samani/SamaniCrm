using FluentValidation;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Host.Models;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using FluentValidation.Validators;

namespace SamaniCrm.Host.Middlewares;


/// <summary>
/// Catches exceptions and formats ApiResponse with errors, meta, and optional details in Development.
/// </summary>
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
        catch (FluentValidation.ValidationException vex)
        {
            await HandleValidationExceptionAsync(context, vex);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, FluentValidation.ValidationException vex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json; charset=utf-8";

        var errors = vex.Errors.Select(e => new ApiError
        {
            Field = e.PropertyName,
            Message = e.ErrorMessage
        }).ToList();

        var response = ApiResponse<object>.Fail(
            errors: errors,
            meta: null
        );

        await WriteResponseAsync(context, response);
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception");
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json; charset=utf-8";

        var apiError = new ApiError
        {
            Message = "An unexpected error occurred."
        };

        // In Development mode, include exception details
        if (_env.IsDevelopment())
        {
            apiError.Detail = ex.ToString();
        }

        var response = ApiResponse<object>.Fail(
            errors: new List<ApiError> { apiError },
            meta: null
        );

        await WriteResponseAsync(context, response);
    }

    private async Task WriteResponseAsync<T>(HttpContext context, ApiResponse<T> response)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var payload = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(payload);
    }
}

