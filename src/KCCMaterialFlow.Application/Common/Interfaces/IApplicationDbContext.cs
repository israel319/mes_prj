using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Entities.Staging;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Abstraction du DbContext â€” l'Application ne connait PAS EF Core impl.
/// ImplÃ©mentÃ© par Infrastructure/Data/ApplicationDbContext.
/// </summary>
public interface IApplicationDbContext
{
    // â”€â”€ BonEntree Aggregate â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    DbSet<BonEntree> BonsEntree { get; }
    DbSet<Materiel> Materiels { get; }
    DbSet<Approbation> Approbations { get; }
    DbSet<ItinerairePrevu> ItinerairesPrevu { get; }
    DbSet<BonEntreeHistory> BonEntreeHistoriques { get; }

    // â”€â”€ BonSortie Aggregate â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    DbSet<BonSortie> BonsSortie { get; }
    DbSet<BonSortieExterne> BonsSortieExterne { get; }
    DbSet<BonSortieInterne> BonsSortieInterne { get; }
    DbSet<Pret> Prets { get; }
    DbSet<MaterielSortie> MaterielsSortie { get; }
    DbSet<ApprobationSortie> ApprobationsSortie { get; }
    DbSet<ItineraireSortie> ItinerairesSortie { get; }
    DbSet<BonSortieHistory> BonSortieHistoriques { get; }

    // â”€â”€ Securite â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    DbSet<ScanEvenement> ScansEvenement { get; }
    DbSet<Anomalie> Anomalies { get; }
    DbSet<HistoriqueScan> HistoriqueScans { get; }

    // â”€â”€ RÃ©fÃ©rence / Admin â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Barriere> Barrieres { get; }
    DbSet<ParametreSysteme> ParametresSysteme { get; }
    DbSet<WorkflowEtapeConfig> WorkflowEtapeConfigs { get; }
    DbSet<Compagnie> Compagnies { get; }
    DbSet<Contrat> Contrats { get; }
    DbSet<Employee> Employees { get; }
    DbSet<AppUser> AppUsers { get; }
    DbSet<WorkflowApprobateurSpecial> WorkflowApprobateursSpeciaux { get; }

    // â”€â”€ Tables tampons (import DATA.xlsx) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    DbSet<StagingCompany> StagingCompanies { get; }
    DbSet<StagingContract> StagingContracts { get; }
    DbSet<CategorieSortie> CategoriesSortie { get; }
    DbSet<RaisonSortie> RaisonsSortie { get; }
    DbSet<Site> Sites { get; }
    DbSet<NotificationRejet> NotificationsRejet { get; }
    DbSet<SoldeMateriel> SoldeMateriels { get; }
    DbSet<Checkpoint> Checkpoints { get; }
    DbSet<PassageCheckpoint> PassagesCheckpoint { get; }
    DbSet<Statut> Statuts { get; }
    DbSet<TypeMaterielEntity> TypesMateriels { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
