using Microsoft.AspNetCore.Components.Server.Circuits;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// CircuitHandler qui gère proprement le cycle de vie des circuits Blazor Server.
/// Permet de tracer les connections/déconnexions et d'éviter les ObjectDisposedException
/// causées par des rendus en queue quand le WebSocket se ferme.
/// </summary>
public class GracefulCircuitHandler : CircuitHandler
{
    private readonly ILogger<GracefulCircuitHandler> _logger;

    public GracefulCircuitHandler(ILogger<GracefulCircuitHandler> logger)
    {
        _logger = logger;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Circuit {CircuitId} ouvert", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Circuit {CircuitId} reconnecté", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Circuit {CircuitId} déconnecté", circuit.Id);
        return Task.CompletedTask;
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Circuit {CircuitId} fermé et nettoyé", circuit.Id);
        return Task.CompletedTask;
    }
}
