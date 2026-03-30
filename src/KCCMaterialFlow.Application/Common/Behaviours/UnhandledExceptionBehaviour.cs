using MediatR;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Application.Common.Behaviours;

/// <summary>
/// Attrape les exceptions non gérées et log avec contexte complet.
/// </summary>
public sealed class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Name}: {@Request}",
                typeof(TRequest).Name, request);
            throw;
        }
    }
}
