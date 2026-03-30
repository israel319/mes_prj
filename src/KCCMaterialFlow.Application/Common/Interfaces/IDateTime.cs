namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Abstraction DateTime pour la testabilité.
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
