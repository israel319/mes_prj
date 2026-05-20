using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ApprobationSortie.
/// </summary>
public class ApprobationSortieConfiguration : IEntityTypeConfiguration<ApprobationSortie>
{
    public void Configure(EntityTypeBuilder<ApprobationSortie> builder)
    {
        builder.ToTable("T_ApprobationsSortie", "dbo");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("IdApprobation")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.BonId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(a => a.OrdreEtape)
            .HasColumnName("OrdreEtape");

        builder.Property(a => a.RoleApprobateur)
            .HasColumnName("RoleCode")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(a => a.NomEtape)
            .HasColumnName("NomEtape")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.CodeEtape)
            .HasColumnName("CodeEtape")
            .HasMaxLength(30);

        builder.Property(a => a.ApprobateurId)
            .HasColumnName("ApprobateurId");

        builder.Property(a => a.ApprobateurMatricule)
            .HasColumnName("ApprobateurMatricule")
            .HasMaxLength(50);

        builder.Property(a => a.Decision)
            .HasColumnName("Decision")
            .HasMaxLength(50)
            .HasDefaultValue("En attente");

        builder.Property(a => a.DateAction)
            .HasColumnName("DateAction");

        builder.Property(a => a.ApprobateurLogin)
            .HasColumnName("ApprobateurLogin")
            .HasMaxLength(100);

        builder.Property(a => a.NomApprobateur)
            .HasColumnName("ApprobateurNom")
            .HasMaxLength(200);

        builder.Property(a => a.ReservesEventuelles)
            .HasColumnName("Commentaire")
            .HasMaxLength(1000);

        // Index sur le bon de sortie
        builder.HasIndex(a => a.BonId)
            .HasDatabaseName("IX_ApprobationsSortie_BonSortieId");

        // Index sur l'ordre d'étape
        builder.HasIndex(a => new { a.BonId, a.OrdreEtape })
            .HasDatabaseName("IX_ApprobationsSortie_Etape");

        // Index RoleCode + Decision : clé de la requête "bons en attente pour mon rôle"
        builder.HasIndex(a => new { a.RoleApprobateur, a.Decision })
            .HasDatabaseName("IX_ApprobationsSortie_RoleCode_Decision");

        // Index clé v2 : "bons en attente pour cet approbateur"
        builder.HasIndex(a => new { a.ApprobateurId, a.Decision })
            .HasDatabaseName("IX_ApprobationsSortie_Approbateur_Decision");
    }
}
