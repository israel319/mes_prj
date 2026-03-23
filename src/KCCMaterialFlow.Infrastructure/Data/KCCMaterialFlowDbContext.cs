using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// DbContext principal de l'application avec support modulaire
/// </summary>
public class KCCMaterialFlowDbContext : DbContext, IAppDbContext
{
    private readonly Assembly[] _moduleAssemblies;
    
    /// <summary>
    /// Assemblies de modules à utiliser pour les configurations EF.
    /// Configuré au démarrage de l'application via Program.cs
    /// </summary>
    public static IEnumerable<Assembly>? ConfiguredModuleAssemblies { get; set; }

    /// <summary>
    /// Notifications de rejet stockées en base
    /// </summary>
    public DbSet<NotificationRejet> NotificationsRejet => Set<NotificationRejet>();

    // ===== TABLES DE RÉFÉRENCE =====
    
    /// <summary>
    /// Liste des compagnies (contractors, sous-traitants)
    /// </summary>
    public DbSet<Compagnie> Compagnies => Set<Compagnie>();

    /// <summary>
    /// Contrats des compagnies
    /// </summary>
    public DbSet<Contrat> Contrats => Set<Contrat>();

    /// <summary>
    /// Liste des départements
    /// </summary>
    public DbSet<Departement> Departements => Set<Departement>();

    /// <summary>
    /// Liste des sites (FROM/TO)
    /// </summary>
    public DbSet<Site> Sites => Set<Site>();

    /// <summary>
    /// Liste des employes (escorteurs, demandeurs)
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// Categories de sortie (Externe, Interne)
    /// </summary>
    public DbSet<CategorieSortie> CategoriesSortie => Set<CategorieSortie>();

    /// <summary>
    /// Raisons de sortie (Fin chantier, Informatique, etc.)
    /// </summary>
    public DbSet<RaisonSortie> RaisonsSortie => Set<RaisonSortie>();

    /// <summary>
    /// Soldes de matériels pour liaison BEM ↔ BSM et gestion du reliquat
    /// </summary>
    public DbSet<SoldeMateriel> SoldesMateriels => Set<SoldeMateriel>();

    /// <summary>
    /// Checkpoints/Barrieres pour le suivi
    /// </summary>
    public DbSet<Checkpoint> Checkpoints => Set<Checkpoint>();

    /// <summary>
    /// Passages aux checkpoints (pour detection anomalies)
    /// </summary>
    public DbSet<PassageCheckpoint> PassagesCheckpoint => Set<PassageCheckpoint>();

    // ===== PERMISSIONS & RÔLES =====

    /// <summary>
    /// Permissions système (CREATE_BON, APPROVE_BON, etc.)
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// Table de liaison rôle-permission
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // ===== ACTIVITÉS =====

    /// <summary>
    /// Activités métier du système (BEM_CREER, BSM_APPROUVER, SEC_SCANNER, etc.)
    /// </summary>
    public DbSet<Activite> Activites => Set<Activite>();

    /// <summary>
    /// Table de liaison utilisateur-activité (assignation d'activités par utilisateur)
    /// </summary>
    public DbSet<UtilisateurActivite> UtilisateurActivites => Set<UtilisateurActivite>();

    /// <summary>
    /// Configuration flexible des étapes de workflow d'approbation (par type de bon et motif)
    /// </summary>
    public DbSet<WorkflowEtapeConfig> WorkflowEtapesConfig => Set<WorkflowEtapeConfig>();

    public KCCMaterialFlowDbContext(DbContextOptions<KCCMaterialFlowDbContext> options)
        : base(options)
    {
        // Utiliser les assemblies configurées globalement si disponibles
        _moduleAssemblies = ConfiguredModuleAssemblies?.ToArray() ?? [];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appliquer les configurations depuis l'assembly Infrastructure (Data/Configurations/)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KCCMaterialFlowDbContext).Assembly);

        // Appliquer les configurations depuis les assemblies des modules
        foreach (var assembly in _moduleAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
        
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
        // Audit automatique des entités
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
            if (entry.Entity is IAuditableEntity auditable)
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
