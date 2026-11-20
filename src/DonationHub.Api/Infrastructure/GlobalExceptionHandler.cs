using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Infrastructure;

/// <summary>
/// Centralized ASP.NET Core exception handler capturing unhandled errors,
/// logging with full trace details, and returning sanitized RFC 9457 ProblemDetails.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(
            exception,
            "Unhandled exception caught during request execution on {Path}. TraceId: {TraceId}",
            httpContext.Request.Path,
            traceId);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.io/500",
            Title = "An unexpected error occurred while processing your request.",
            Detail = null // Never leak database details, SQL statements, or stack traces
        };

        problemDetails.Extensions["code"] = "Server.UnhandledException";
        problemDetails.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
