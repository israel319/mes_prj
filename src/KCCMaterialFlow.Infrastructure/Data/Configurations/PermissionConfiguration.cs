using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Permission
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("T_Permissions", "dbo");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("IdPermission").ValueGeneratedOnAdd();

        builder.Property(p => p.CodePermission)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.NomPermission)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Categorie)
            .HasMaxLength(50);

        builder.Property(p => p.DateCreation)
            .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(p => p.CodePermission)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_CodePermission");

        builder.HasIndex(p => p.Categorie)
            .HasDatabaseName("IX_Permissions_Categorie");

        builder.HasIndex(p => p.EstActif)
            .HasDatabaseName("IX_Permissions_EstActif");

        // Données de base pour les permissions système
        builder.HasData(
            // === Catégorie: Bons ===
            new Permission
            {
                Id =1,
                CodePermission = "ALL",
                NomPermission = "Accès complet",
                Description = "Accès complet à toutes les fonctionnalités",
                Categorie = "Système",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 0,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =2,
                CodePermission = "CREATE_BON",
                NomPermission = "Créer des bons",
                Description = "Créer des bons d'entrée/sortie de matériel",
                Categorie = "Bons",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 10,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =3,
                CodePermission = "VIEW_BON",
                NomPermission = "Voir tous les bons",
                Description = "Consulter tous les bons du système",
                Categorie = "Bons",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 20,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =4,
                CodePermission = "VIEW_OWN_BON",
                NomPermission = "Voir ses propres bons",
                Description = "Consulter uniquement ses propres bons",
                Categorie = "Bons",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 25,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =5,
                CodePermission = "APPROVE_BON",
                NomPermission = "Approuver les bons",
                Description = "Approuver les bons de son département",
                Categorie = "Bons",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 30,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =6,
                CodePermission = "REJECT_BON",
                NomPermission = "Rejeter les bons",
                Description = "Rejeter les bons de son département",
                Categorie = "Bons",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 35,
                DateCreation = new DateTime(2024, 1, 1)
            },
            // === Catégorie: Sécurité ===
            new Permission
            {
                Id =7,
                CodePermission = "SCAN_BON",
                NomPermission = "Scanner les QR codes",
                Description = "Scanner les QR codes des bons aux barrières",
                Categorie = "Sécurité",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 40,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =8,
                CodePermission = "CREATE_ANOMALIE",
                NomPermission = "Signaler des anomalies",
                Description = "Créer des signalements d'anomalies",
                Categorie = "Sécurité",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 45,
                DateCreation = new DateTime(2024, 1, 1)
            },
            // === Catégorie: Rapports ===
            new Permission
            {
                Id =9,
                CodePermission = "VIEW_REPORTS",
                NomPermission = "Accéder aux rapports",
                Description = "Consulter les rapports et statistiques",
                Categorie = "Rapports",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 50,
                DateCreation = new DateTime(2024, 1, 1)
            },
            // === Catégorie: Administration ===
            new Permission
            {
                Id =10,
                CodePermission = "MANAGE_USERS",
                NomPermission = "Gérer les utilisateurs",
                Description = "Ajouter, modifier et désactiver les utilisateurs",
                Categorie = "Administration",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 60,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Permission
            {
                Id =11,
                CodePermission = "MANAGE_SETTINGS",
                NomPermission = "Gérer les paramètres",
                Description = "Modifier les paramètres système",
                Categorie = "Administration",
                EstActif = true,
                EstSysteme = true,
                OrdreAffichage = 70,
                DateCreation = new DateTime(2024, 1, 1)
            }
        );
    }
}

/// <summary>
/// Configuration Entity Framework pour l'entité RolePermission
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("T_RolePermissions", "dbo");

        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).HasColumnName("IdRolePermission").ValueGeneratedOnAdd();

        builder.HasIndex(rp => new { rp.IdRole, rp.IdPermission })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_Role_Permission");

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.IdRole)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.IdPermission)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed : attribution des permissions aux rôles système
        builder.HasData(
            // ADMIN (IdRole=1) → ALL (IdPermission=1)
            new RolePermission { Id = 1, IdRole = 1, IdPermission = 1, DateAttribution = new DateTime(2024, 1, 1) },

            // APPROBATEUR (IdRole=2) → VIEW_BON, APPROVE_BON, REJECT_BON
            new RolePermission { Id = 2, IdRole = 2, IdPermission = 3, DateAttribution = new DateTime(2024, 1, 1) },
            new RolePermission { Id = 3, IdRole = 2, IdPermission = 5, DateAttribution = new DateTime(2024, 1, 1) },
            new RolePermission { Id = 4, IdRole = 2, IdPermission = 6, DateAttribution = new DateTime(2024, 1, 1) },

            // AGENT_SECURITE (IdRole=3) → VIEW_BON, SCAN_BON, CREATE_ANOMALIE
            new RolePermission { Id = 5, IdRole = 3, IdPermission = 3, DateAttribution = new DateTime(2024, 1, 1) },
            new RolePermission { Id = 6, IdRole = 3, IdPermission = 7, DateAttribution = new DateTime(2024, 1, 1) },
            new RolePermission { Id = 7, IdRole = 3, IdPermission = 8, DateAttribution = new DateTime(2024, 1, 1) },

            // UTILISATEUR (IdRole=4) → CREATE_BON, VIEW_OWN_BON
            new RolePermission { Id = 8, IdRole = 4, IdPermission = 2, DateAttribution = new DateTime(2024, 1, 1) },
            new RolePermission { Id = 9, IdRole = 4, IdPermission = 4, DateAttribution = new DateTime(2024, 1, 1) }
        );
    }
}
