using MediatR;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Application.Common.Behaviours;

/// <summary>
/// Log automatique de chaque request MediatR avec timing.
/// Si > 500ms, log un warning.
/// </summary>
public sealed class LoggingBehaviour<TRequest, TResponse>(
    ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        logger.LogInformation("[Start] {Name}", name);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > 500)
            logger.LogWarning("[Slow] {Name} took {Elapsed}ms", name, sw.ElapsedMilliseconds);
        else
            logger.LogInformation("[End] {Name} in {Elapsed}ms", name, sw.ElapsedMilliseconds);

        return response;
    }
}
