using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Entities.Staging;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// DbContext principal de l'application â€” implÃ©mente IApplicationDbContext (Clean Architecture).
/// Toutes les configurations EF sont dans Infrastructure/Data/Configurations/.
/// </summary>
public class KCCMaterialFlowDbContext : DbContext, IApplicationDbContext
{

    // â”€â”€ BonEntree Aggregate â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public DbSet<BonEntree> BonsEntree => Set<BonEntree>();
    public DbSet<Materiel> Materiels => Set<Materiel>();
    public DbSet<Approbation> Approbations => Set<Approbation>();
    public DbSet<ItinerairePrevu> ItinerairesPrevu => Set<ItinerairePrevu>();
    public DbSet<BonEntreeHistory> BonEntreeHistoriques => Set<BonEntreeHistory>();

    // â”€â”€ BonSortie Aggregate â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public DbSet<BonSortie> BonsSortie => Set<BonSortie>();
    public DbSet<BonSortieExterne> BonsSortieExterne => Set<BonSortieExterne>();
    public DbSet<BonSortieInterne> BonsSortieInterne => Set<BonSortieInterne>();
    public DbSet<Pret> Prets => Set<Pret>();
    public DbSet<MaterielSortie> MaterielsSortie => Set<MaterielSortie>();
    public DbSet<ApprobationSortie> ApprobationsSortie => Set<ApprobationSortie>();
    public DbSet<ItineraireSortie> ItinerairesSortie => Set<ItineraireSortie>();
    public DbSet<BonSortieHistory> BonSortieHistoriques => Set<BonSortieHistory>();

    // â”€â”€ Securite â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public DbSet<ScanEvenement> ScansEvenement => Set<ScanEvenement>();
    public DbSet<Anomalie> Anomalies => Set<Anomalie>();
    public DbSet<HistoriqueScan> HistoriqueScans => Set<HistoriqueScan>();

    // â”€â”€ RÃ©fÃ©rence / Admin â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Barriere> Barrieres => Set<Barriere>();
    public DbSet<ParametreSysteme> ParametresSysteme => Set<ParametreSysteme>();
    public DbSet<WorkflowEtapeConfig> WorkflowEtapeConfigs => Set<WorkflowEtapeConfig>();
    public DbSet<Compagnie> Compagnies => Set<Compagnie>();
    public DbSet<Contrat> Contrats => Set<Contrat>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AllEmployee> AllEmployees => Set<AllEmployee>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<WorkflowApprobateurSpecial> WorkflowApprobateursSpeciaux => Set<WorkflowApprobateurSpecial>();

    // â”€â”€ Tables tampons (import DATA.xlsx) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public DbSet<StagingCompany> StagingCompanies => Set<StagingCompany>();
    public DbSet<StagingContract> StagingContracts => Set<StagingContract>();
    public DbSet<CategorieSortie> CategoriesSortie => Set<CategorieSortie>();
    public DbSet<RaisonSortie> RaisonsSortie => Set<RaisonSortie>();
    public DbSet<RaisonEntree> RaisonsEntree => Set<RaisonEntree>();
    public DbSet<RaisonEntreeRaisonSortie> RaisonEntreeRaisonsSortie => Set<RaisonEntreeRaisonSortie>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<NotificationRejet> NotificationsRejet => Set<NotificationRejet>();
    public DbSet<SoldeMateriel> SoldeMateriels => Set<SoldeMateriel>();
    public DbSet<Checkpoint> Checkpoints => Set<Checkpoint>();
    public DbSet<PassageCheckpoint> PassagesCheckpoint => Set<PassageCheckpoint>();
    public DbSet<Statut> Statuts => Set<Statut>();
    public DbSet<TypeMaterielEntity> TypesMateriels => Set<TypeMaterielEntity>();

    public KCCMaterialFlowDbContext(DbContextOptions<KCCMaterialFlowDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Toutes les configurations sont dans Infrastructure/Data/Configurations/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KCCMaterialFlowDbContext).Assembly);

        // Configuration globale par dÃ©faut
        ConfigureConventions(modelBuilder);
    }

    /// <summary>
    /// Conventions globales pour toutes les entitÃ©s
    /// </summary>
    private static void ConfigureConventions(ModelBuilder modelBuilder)
    {
        // Toutes les propriÃ©tÃ©s string ont une longueur max par dÃ©faut
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties()
                .Where(p => p.ClrType == typeof(string)))
            {
                if (property.GetMaxLength() == null)
                {
                    property.SetMaxLength(500);
                }
            }

            // Convention de nommage pour les tables
            if (entity.GetTableName() is { } tableName)
            {
                entity.SetTableName(tableName);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Met Ã  jour automatiquement les champs d'audit (CreatedAt, UpdatedAt)
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseAuditableEntity auditable)
            {
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                }

                auditable.UpdatedAt = now;
            }
        }
    }
}
