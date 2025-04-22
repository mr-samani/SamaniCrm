using System.Net;
using System.Text.Json;
using FluentValidation;

namespace SamaniCrm.Host.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                errors = ex.Errors
                    .Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                errors = new[]
                {
                    new { field = "", message = "An unexpected error occurred." }
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
