using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace eCommerceSolution.OrdersService.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            // Await the next delegate so exceptions thrown asynchronously are caught here
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError($"An error occurred: {ex.Message}, Exception Type: {ex.GetType()}");

            if (ex.InnerException is not null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }

            // If the response has already started we can't modify it. Re-throw so the server can handle it.
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the exception handling middleware cannot modify the response.");
                throw;
            }

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later."
            });
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
