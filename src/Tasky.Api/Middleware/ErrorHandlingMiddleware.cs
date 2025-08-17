using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Tasky.Api.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            ctx.Response.ContentType = "application/problem+json";

            var pd = new ProblemDetails
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            pd.Extensions["errors"] = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

            await ctx.Response.WriteAsJsonAsync(pd);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/problem+json";

            var pd = new ProblemDetails
            {
                Title = "Server error",
                Status = StatusCodes.Status500InternalServerError
            };
            await ctx.Response.WriteAsJsonAsync(pd);
        }
    }
}
