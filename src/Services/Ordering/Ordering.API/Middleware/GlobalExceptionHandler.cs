using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ordering.API.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            ValidationException => 
                (HttpStatusCode.BadRequest, "Validation Error", "One or more validation failures occurred."),
            Microsoft.Data.SqlClient.SqlException => 
                (HttpStatusCode.ServiceUnavailable, "Database Error", "The database is unreachable."),
            _ => 
                (HttpStatusCode.InternalServerError, "Server Error", "An unexpected error occurred.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.io/{(int)statusCode}"
        };

        if (exception is ValidationException validationException)
        {
            var validationError = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select( e => e.ErrorMessage).ToArray()
                );

            problemDetails.Extensions.Add("errors", validationError);
        }

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}