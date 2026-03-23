namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Interface de base pour toutes les entités avec un identifiant
/// </summary>
public interface IEntity
{
    int Id { get; set; }
}

/// <summary>
/// Interface de base pour les entités avec clé primaire nommée IdBon
/// (utilisée par les bons de sortie)
/// </summary>
public interface IBonEntity
{
    int IdBon { get; set; }
}
