using ReserveIt.BLL.Exceptions;
using System.ComponentModel.DataAnnotations;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ReserveIt.Presentation.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message = exception.Message,
                detail = exception is ApiException ? null : "An error occurred on the server."
            }
        };

        switch (exception)
        {
            case BLL.Exceptions.ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new
                {
                    error = new
                    {
                        message = "Validation error",
                        detail = string.Join(" ", validationException.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"))
                    }
                }!;
                break;

            case ApiException apiException:
                context.Response.StatusCode = apiException.StatusCode;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}