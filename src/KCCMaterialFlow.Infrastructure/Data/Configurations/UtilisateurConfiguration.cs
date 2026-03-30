using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Utilisateur
/// </summary>
public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> builder)
    {
        builder.ToTable("T_Utilisateurs", "dbo");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("IdUtilisateur")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Login)
            .HasColumnName("Login")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.NomComplet)
            .HasColumnName("NomComplet")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Fonction)
            .HasColumnName("Fonction")
            .HasMaxLength(150);

        builder.Property(u => u.Departement)
            .HasColumnName("Departement")
            .HasMaxLength(100);

        builder.Property(u => u.IdRole)
            .HasColumnName("IdRole")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(200);

        builder.Property(u => u.Telephone)
            .HasColumnName("Telephone")
            .HasMaxLength(50);

        builder.Property(u => u.EstActif)
            .HasColumnName("EstActif")
            .HasDefaultValue(true);

        builder.Property(u => u.DateCreation)
            .HasColumnName("DateCreation")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.DateModification)
            .HasColumnName("DateModification");

        builder.Property(u => u.DerniereConnexion)
            .HasColumnName("DerniereConnexion");

        // Index unique sur le login
        builder.HasIndex(u => u.Login)
            .IsUnique()
            .HasDatabaseName("IX_Utilisateurs_Login");

        // Index sur le département pour les requêtes de filtrage
        builder.HasIndex(u => u.Departement)
            .HasDatabaseName("IX_Utilisateurs_Departement");

        // Index sur EstActif pour filtrer les utilisateurs actifs
        builder.HasIndex(u => u.EstActif)
            .HasDatabaseName("IX_Utilisateurs_EstActif");

        // Index sur IdRole
        builder.HasIndex(u => u.IdRole)
            .HasDatabaseName("IX_Utilisateurs_IdRole");

        // Relation Utilisateur -> Role (FK)
        builder.HasOne(u => u.RolePrincipal)
            .WithMany()
            .HasForeignKey(u => u.IdRole)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);
    }
}
