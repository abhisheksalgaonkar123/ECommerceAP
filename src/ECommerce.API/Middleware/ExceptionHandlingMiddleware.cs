using System.Net;
using System.Text.Json;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Domain.Exceptions;
using FluentValidation;

namespace ECommerce.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            await HandleExceptionAsync(
                context,
                HttpStatusCode.BadRequest,
                errors);
        }
        catch (DomainException ex)
        {
            await HandleExceptionAsync(
                context,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (Exception ex)
        {
            // Log real error — but don't expose to client!
            _logger.LogError(ex,
                "Unexpected error occurred: {Message}",
                ex.Message);

            await HandleExceptionAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        object error)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = context.Response.StatusCode,
            error = error
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
