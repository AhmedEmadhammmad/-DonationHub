using System.Diagnostics;
using DonationHub.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Extensions;

/// <summary>
/// Maps Result and Result&lt;T&gt; instances directly to standard ASP.NET Core ActionResults
/// and RFC 9457 ProblemDetails representations upon failure.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return CreateProblemResult(result.Error, controller);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return CreateProblemResult(result.Error, controller);
    }

    public static IActionResult ToCreatedAtActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string actionName,
        object routeValues)
    {
        if (result.IsSuccess)
        {
            return controller.CreatedAtAction(actionName, routeValues, result.Value);
        }

        return CreateProblemResult(result.Error, controller);
    }

    private static IActionResult CreateProblemResult(Error error, ControllerBase controller)
    {
        var statusCode = (int)error.Type;
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = error.Message
        };

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = traceId;

        if (error.ValidationErrors is not null && error.ValidationErrors.Count > 0)
        {
            problemDetails.Extensions["errors"] = error.ValidationErrors;
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
