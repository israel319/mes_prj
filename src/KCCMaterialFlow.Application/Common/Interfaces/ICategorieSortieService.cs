using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface ICategorieSortieService
{
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesAsync();
    Task<CategorieSortie?> GetCategorieByIdAsync(int id);
    Task<CategorieSortie?> GetCategorieByCodeAsync(string code);
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieIdAsync(int categorieId);
    Task<IEnumerable<RaisonSortie>> GetRaisonsByCategorieCodeAsync(string categorieCode);
    Task<RaisonSortie?> GetRaisonByIdAsync(int id);
    Task<IEnumerable<CategorieSortie>> GetAllCategoriesWithRaisonsAsync();

    // ── Mapping Département → Raisons autorisées ──

    /// <summary>
    /// Retourne les raisons de sortie autorisées pour un département donné.
    /// Si aucun mapping spécifique n'existe, retourne les raisons par défaut (DepartementId=NULL).
    /// </summary>
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsAutoriseesByDepartementAsync(string departementCode);

    /// <summary>
    /// Retourne les raisons de sortie autorisées pour un département donné par son ID.
    /// Si departementId est null, retourne les raisons par défaut.
    /// </summary>
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsAutoriseesByDepartementIdAsync(int? departementId);

    /// <summary>
    /// Retourne le code du département qui requiert spécifiquement cette raison de sortie.
    /// Retourne null si la raison fait partie du mapping par défaut.
    /// </summary>
    Task<string?> GetDepartementCodeForRaisonAsync(string raisonSortieCode);

    /// <summary>
    /// Retourne les mappings pour un département (ou les défauts si departementId=null).
    /// </summary>
    Task<IReadOnlyList<DepartementRaisonSortie>> GetDepartementRaisonMappingsAsync(int? departementId);

    /// <summary>
    /// Sauvegarde les mappings pour un département donné.
    /// </summary>
    Task SaveDepartementRaisonMappingsAsync(int? departementId, List<int> raisonSortieIds);
}
