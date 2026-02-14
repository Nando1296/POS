using System.Net;
using System.Text.Json;
using Ordering.Domain.Exceptions;
using FluentValidation;

namespace Ordering.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Exception caught in Order Service Middleware.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            OrderNotFoundException =>(HttpStatusCode.NotFound, ex.Message),
            ValidationException => (HttpStatusCode.BadRequest, "Validation failed."),
            InvalidOrderDataException => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOrderStateException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        object responsePayload;

        if(ex is ValidationException validationEx)
        {
            responsePayload = new
            {
                error = message,
                timestamp = DateTime.UtcNow,
                details = validationEx.Errors.Select(e => new
                {
                    e.PropertyName, e.ErrorMessage 
                })
            };
        }
        else
        {
            responsePayload = new
            {
                error = message,
                timestamp = DateTime.UtcNow,
                type = ex.GetType().Name
            };
        }

        var result = JsonSerializer.Serialize(responsePayload);
        await context.Response.WriteAsync(result);
    }
}