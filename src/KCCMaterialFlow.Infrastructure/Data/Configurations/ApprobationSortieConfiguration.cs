using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.BonEntree.Entities;

using KCCMaterialFlow.Module.BonSortie.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ApprobationSortie.
/// </summary>
public class ApprobationSortieConfiguration : IEntityTypeConfiguration<ApprobationSortie>
{
    public void Configure(EntityTypeBuilder<ApprobationSortie> builder)
    {
        builder.ToTable("T_ApprobationsSortie", "dbo");

        builder.HasKey(a => a.IdApprobation);

        builder.Property(a => a.IdApprobation)
            .HasColumnName("IdApprobation")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.BonSortieId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(a => a.OrdreEtape)
            .HasColumnName("OrdreEtape");

        builder.Property(a => a.RoleCode)
            .HasColumnName("RoleCode")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(a => a.NomEtape)
            .HasColumnName("NomEtape")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Decision)
            .HasColumnName("Decision")
            .HasMaxLength(50)
            .HasDefaultValue("En attente");

        builder.Property(a => a.DateAction)
            .HasColumnName("DateAction");

        builder.Property(a => a.ApprobateurLogin)
            .HasColumnName("ApprobateurLogin")
            .HasMaxLength(100);

        builder.Property(a => a.ApprobateurNom)
            .HasColumnName("ApprobateurNom")
            .HasMaxLength(200);

        builder.Property(a => a.Commentaire)
            .HasColumnName("Commentaire")
            .HasMaxLength(1000);

        // Index sur le bon de sortie
        builder.HasIndex(a => a.BonSortieId)
            .HasDatabaseName("IX_ApprobationsSortie_BonSortieId");

        // Index sur l'ordre d'étape
        builder.HasIndex(a => new { a.BonSortieId, a.OrdreEtape })
            .HasDatabaseName("IX_ApprobationsSortie_Etape");

        // Index RoleCode + Decision : clé de la requête "bons en attente pour mon rôle"
        builder.HasIndex(a => new { a.RoleCode, a.Decision })
            .HasDatabaseName("IX_ApprobationsSortie_RoleCode_Decision");
    }
}
