using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// DbContext principal de l'application — implémente IApplicationDbContext (Clean Architecture).
/// Toutes les configurations EF sont dans Infrastructure/Data/Configurations/.
/// </summary>
public class KCCMaterialFlowDbContext : DbContext, IApplicationDbContext
{

    // ── BonEntree Aggregate ─────────────────────────────────────────
    public DbSet<BonEntree> BonsEntree => Set<BonEntree>();
    public DbSet<Materiel> Materiels => Set<Materiel>();
    public DbSet<Approbation> Approbations => Set<Approbation>();
    public DbSet<ItinerairePrevu> ItinerairesPrevu => Set<ItinerairePrevu>();
    public DbSet<BonEntreeHistory> BonEntreeHistoriques => Set<BonEntreeHistory>();

    // ── BonSortie Aggregate ─────────────────────────────────────────
    public DbSet<BonSortie> BonsSortie => Set<BonSortie>();
    public DbSet<BonSortieExterne> BonsSortieExterne => Set<BonSortieExterne>();
    public DbSet<BonSortieInterne> BonsSortieInterne => Set<BonSortieInterne>();
    public DbSet<Pret> Prets => Set<Pret>();
    public DbSet<MaterielSortie> MaterielsSortie => Set<MaterielSortie>();
    public DbSet<ApprobationSortie> ApprobationsSortie => Set<ApprobationSortie>();
    public DbSet<ItineraireSortie> ItinerairesSortie => Set<ItineraireSortie>();
    public DbSet<BonSortieHistory> BonSortieHistoriques => Set<BonSortieHistory>();

    // ── Securite ────────────────────────────────────────────────────
    public DbSet<ScanEvenement> ScansEvenement => Set<ScanEvenement>();
    public DbSet<Anomalie> Anomalies => Set<Anomalie>();
    public DbSet<HistoriqueScan> HistoriqueScans => Set<HistoriqueScan>();

    // ── Référence / Admin ───────────────────────────────────────────
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Barriere> Barrieres => Set<Barriere>();
    public DbSet<ParametreSysteme> ParametresSysteme => Set<ParametreSysteme>();
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UtilisateurRole> UtilisateurRoles => Set<UtilisateurRole>();
    public DbSet<Departement> Departements => Set<Departement>();
    public DbSet<WorkflowEtapeConfig> WorkflowEtapeConfigs => Set<WorkflowEtapeConfig>();
    public DbSet<Activite> Activites => Set<Activite>();
    public DbSet<UtilisateurActivite> UtilisateurActivites => Set<UtilisateurActivite>();
    public DbSet<Compagnie> Compagnies => Set<Compagnie>();
    public DbSet<Contrat> Contrats => Set<Contrat>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<CategorieSortie> CategoriesSortie => Set<CategorieSortie>();
    public DbSet<RaisonSortie> RaisonsSortie => Set<RaisonSortie>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<NotificationRejet> NotificationsRejet => Set<NotificationRejet>();
    public DbSet<SoldeMateriel> SoldeMateriels => Set<SoldeMateriel>();
    public DbSet<Checkpoint> Checkpoints => Set<Checkpoint>();
    public DbSet<PassageCheckpoint> PassagesCheckpoint => Set<PassageCheckpoint>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
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

        // Configuration globale par défaut
        ConfigureConventions(modelBuilder);
    }

    /// <summary>
    /// Conventions globales pour toutes les entités
    /// </summary>
    private static void ConfigureConventions(ModelBuilder modelBuilder)
    {
        // Toutes les propriétés string ont une longueur max par défaut
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
    /// Met à jour automatiquement les champs d'audit (CreatedAt, UpdatedAt)
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
