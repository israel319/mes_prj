using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IReferenceDataService
{
    Task<IEnumerable<Compagnie>> GetCompagniesAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<Compagnie?> GetCompagnieByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Compagnie?> GetCompagnieByNomAsync(string nom, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contrat>> GetContratsAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contrat>> GetContratsByCompagnieAsync(int compagnieId, bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<Contrat?> GetContratByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Departement>> GetDepartementsAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<Departement?> GetDepartementByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetSitesAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetSitesInternesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetSitesExternesAsync(CancellationToken cancellationToken = default);
    Task<Site?> GetSiteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetEmployeesAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetEscorteursAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetEmployeesByCompagnieAsync(int compagnieId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetEmployeesByDepartementAsync(int departementId, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeByMatriculeAsync(string matricule, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm, int maxResults = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<Compagnie>> SearchCompagniesAsync(string searchTerm, int maxResults = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategorieSortie>> GetCategoriesSortieAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<CategorieSortie?> GetCategorieSortieByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategorieSortie?> GetCategorieSortieByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<RaisonSortie>> GetRaisonsSortieAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<RaisonSortie>> GetRaisonsSortieByCategorieAsync(int categorieId, CancellationToken cancellationToken = default);
    Task<RaisonSortie?> GetRaisonSortieByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Checkpoint>> GetCheckpointsAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<Checkpoint?> GetCheckpointByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Checkpoint?> GetCheckpointBySiteIdAsync(int siteId, CancellationToken cancellationToken = default);
}
