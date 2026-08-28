using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Yggdrasil.Application.Exceptions;

namespace Yggdrasil.Api.Handlers;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        (int statusCode, string title, string detail) = exception switch
        {
            // Can add more custom exceptions here. pls sort them by status code
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                "The request could not be read. Verify it's valid JSON"
            ),
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "one or more fields are invalid"
            ),
            ForbiddenException ex => (StatusCodes.Status403Forbidden, ex.Title, ex.Message),
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Title, ex.Message),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Title, ex.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An Unexpected error occured."
            ),
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception for {Path}", httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        if (exception is ValidationException validation)
        {
            problemDetails.Extensions["errors"] = validation
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
