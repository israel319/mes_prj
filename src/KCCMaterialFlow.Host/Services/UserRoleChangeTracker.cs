using System.Collections.Concurrent;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Singleton : trace les changements de rôle/accès effectués par un administrateur.
/// Permet aux circuits Blazor actifs de détecter qu'ils doivent se recharger
/// pour obtenir des claims frais et un accès immédiatement à jour.
/// </summary>
public sealed class UserRoleChangeTracker
{
    private readonly ConcurrentDictionary<string, DateTime> _changes =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Marque le matricule comme ayant eu un changement de rôle/accès.</summary>
    public void MarkChanged(string? matricule)
    {
        if (!string.IsNullOrWhiteSpace(matricule))
            _changes[matricule.ToUpperInvariant()] = DateTime.UtcNow;
    }

    /// <summary>
    /// Retourne l'horodatage du dernier changement pour ce matricule, ou null si aucun.
    /// </summary>
    public DateTime? GetLastChange(string? matricule)
    {
        if (string.IsNullOrWhiteSpace(matricule)) return null;
        return _changes.TryGetValue(matricule.ToUpperInvariant(), out var dt) ? dt : null;
    }
}
