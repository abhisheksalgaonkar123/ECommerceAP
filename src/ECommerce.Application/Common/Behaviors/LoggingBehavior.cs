using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ECommerce.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Log BEFORE — handler is starting
        _logger.LogInformation(
            "Handling {RequestName}", requestName);

        // Start timer
        var stopwatch = Stopwatch.StartNew();

        // Run handler
        var response = await next();

        // Stop timer
        stopwatch.Stop();

        // Log AFTER — handler finished + how long it took
        _logger.LogInformation(
            "Handled {RequestName} in {ElapsedMs}ms",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
