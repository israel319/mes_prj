using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Approbation.
/// Simplifié selon le diagramme de classe (4 champs principaux).
/// </summary>
public class ApprobationConfiguration : IEntityTypeConfiguration<Approbation>
{
    public void Configure(EntityTypeBuilder<Approbation> builder)
    {
        builder.ToTable("T_Approbations", "dbo");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("IdApprobation")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.BonId)
            .HasColumnName("BonId")
            .IsRequired();

        builder.Property(a => a.OrdreEtape)
            .HasColumnName("OrdreEtape")
            .IsRequired();

        builder.Property(a => a.Decision)
            .HasColumnName("Decision")
            .HasMaxLength(50)
            .HasDefaultValue("En attente");

        builder.Property(a => a.DateAction)
            .HasColumnName("DateAction");

        builder.Property(a => a.ReservesEventuelles)
            .HasColumnName("ReservesEventuelles")
            .HasMaxLength(1000);

        // Relation avec le Bon (définie dans BonConfiguration)
        // Ne pas redéfinir ici pour éviter les conflits

        // Index sur le bon pour récupérer l'historique d'approbation
        builder.HasIndex(a => a.BonId)
            .HasDatabaseName("IX_Approbations_Bon");

        // Index sur la décision pour filtrer les en attente
        builder.HasIndex(a => a.Decision)
            .HasDatabaseName("IX_Approbations_Decision");

        // Index unique pour éviter les doublons d'étape par bon
        builder.HasIndex(a => new { a.BonId, a.OrdreEtape })
            .IsUnique()
            .HasDatabaseName("IX_Approbations_Bon_Etape");
    }
}
