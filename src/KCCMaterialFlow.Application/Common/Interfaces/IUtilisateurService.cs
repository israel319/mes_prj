using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IUtilisateurService
{
    Task<Utilisateur?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> GetByDepartementAsync(string departement, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Utilisateur?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<Utilisateur> UpsertAsync(Utilisateur utilisateur, CancellationToken cancellationToken = default);
    Task DeleteAsync(int utilisateurId, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(string login, CancellationToken cancellationToken = default);
    void InvalidateCache();
}
