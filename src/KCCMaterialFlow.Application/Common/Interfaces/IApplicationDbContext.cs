using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Abstraction du DbContext — l'Application ne connait PAS EF Core impl.
/// Implémenté par Infrastructure/Data/ApplicationDbContext.
/// </summary>
public interface IApplicationDbContext
{
    // ── BonEntree Aggregate ─────────────────────────────────────────
    DbSet<BonEntree> BonsEntree { get; }
    DbSet<Materiel> Materiels { get; }
    DbSet<Approbation> Approbations { get; }
    DbSet<ItinerairePrevu> ItinerairesPrevu { get; }
    DbSet<BonEntreeHistory> BonEntreeHistoriques { get; }

    // ── BonSortie Aggregate ─────────────────────────────────────────
    DbSet<BonSortie> BonsSortie { get; }
    DbSet<BonSortieExterne> BonsSortieExterne { get; }
    DbSet<BonSortieInterne> BonsSortieInterne { get; }
    DbSet<Pret> Prets { get; }
    DbSet<MaterielSortie> MaterielsSortie { get; }
    DbSet<ApprobationSortie> ApprobationsSortie { get; }
    DbSet<ItineraireSortie> ItinerairesSortie { get; }
    DbSet<BonSortieHistory> BonSortieHistoriques { get; }

    // ── Securite ────────────────────────────────────────────────────
    DbSet<ScanEvenement> ScansEvenement { get; }
    DbSet<Anomalie> Anomalies { get; }
    DbSet<HistoriqueScan> HistoriqueScans { get; }

    // ── Référence / Admin ───────────────────────────────────────────
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Barriere> Barrieres { get; }
    DbSet<ParametreSysteme> ParametresSysteme { get; }
    DbSet<Utilisateur> Utilisateurs { get; }
    DbSet<Role> Roles { get; }
    DbSet<UtilisateurRole> UtilisateurRoles { get; }
    DbSet<Departement> Departements { get; }
    DbSet<WorkflowEtapeConfig> WorkflowEtapeConfigs { get; }
    DbSet<Activite> Activites { get; }
    DbSet<UtilisateurActivite> UtilisateurActivites { get; }
    DbSet<Compagnie> Compagnies { get; }
    DbSet<Contrat> Contrats { get; }
    DbSet<Employee> Employees { get; }
    DbSet<CategorieSortie> CategoriesSortie { get; }
    DbSet<RaisonSortie> RaisonsSortie { get; }
    DbSet<Site> Sites { get; }
    DbSet<NotificationRejet> NotificationsRejet { get; }
    DbSet<SoldeMateriel> SoldeMateriels { get; }
    DbSet<Checkpoint> Checkpoints { get; }
    DbSet<PassageCheckpoint> PassagesCheckpoint { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Statut> Statuts { get; }
    DbSet<TypeMaterielEntity> TypesMateriels { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
