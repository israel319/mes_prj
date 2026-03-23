using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Role
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("T_Roles", "dbo");

        builder.HasKey(r => r.IdRole);

        builder.Property(r => r.CodeRole)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.NomRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.HasIndex(r => r.CodeRole)
            .IsUnique()
            .HasDatabaseName("IX_Roles_CodeRole");

        builder.HasIndex(r => r.EstActif)
            .HasDatabaseName("IX_Roles_EstActif");

        // Données de base pour les rôles système
        builder.HasData(
            new Role
            {
                IdRole = 1,
                CodeRole = "ADMIN",
                NomRole = "Administrateur",
                Description = "Accès complet au système",
                NiveauPriorite = 100,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Role
            {
                IdRole = 2,
                CodeRole = "APPROBATEUR",
                NomRole = "Approbateur",
                Description = "Peut approuver les bons de son département",
                NiveauPriorite = 50,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Role
            {
                IdRole = 3,
                CodeRole = "AGENT_SECURITE",
                NomRole = "Agent de sécurité",
                Description = "Peut scanner et contrôler les entrées/sorties",
                NiveauPriorite = 40,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Role
            {
                IdRole = 4,
                CodeRole = "UTILISATEUR",
                NomRole = "Utilisateur",
                Description = "Utilisateur standard - peut créer des bons",
                NiveauPriorite = 10,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            }
        );
    }
}

/// <summary>
/// Configuration Entity Framework pour l'entité UtilisateurRole
/// </summary>
public class UtilisateurRoleConfiguration : IEntityTypeConfiguration<UtilisateurRole>
{
    public void Configure(EntityTypeBuilder<UtilisateurRole> builder)
    {
        builder.ToTable("T_UtilisateurRoles", "dbo");

        builder.HasKey(ur => ur.IdUtilisateurRole);

        builder.Property(ur => ur.AttribueParLogin)
            .HasMaxLength(100);

        builder.HasIndex(ur => new { ur.IdUtilisateur, ur.IdRole })
            .IsUnique()
            .HasDatabaseName("IX_UtilisateurRoles_Utilisateur_Role");

        builder.HasOne(ur => ur.Utilisateur)
            .WithMany(u => u.UtilisateurRoles)
            .HasForeignKey(ur => ur.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UtilisateurRoles)
            .HasForeignKey(ur => ur.IdRole)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
