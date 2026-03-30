using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ItinerairePrevu.
/// Simplifié selon le diagramme de classe (2 champs principaux).
/// </summary>
public class ItinerairePrevuConfiguration : IEntityTypeConfiguration<ItinerairePrevu>
{
    public void Configure(EntityTypeBuilder<ItinerairePrevu> builder)
    {
        builder.ToTable("T_ItinerairesPrevu", "dbo");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("IdItineraire")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.BonId)
            .HasColumnName("BonId")
            .IsRequired();

        builder.Property(i => i.OrdrePassage)
            .HasColumnName("OrdrePassage")
            .IsRequired();

        builder.Property(i => i.BarriereId)
            .HasColumnName("BarriereId")
            .IsRequired();

        // Relation avec le Bon (définie dans BonConfiguration)
        // Ne pas redéfinir ici pour éviter les conflits

        // Index sur le bon pour récupérer l'itinéraire complet
        builder.HasIndex(i => i.BonId)
            .HasDatabaseName("IX_ItinerairesPrevu_Bon");

        // Index sur la barrière pour les requêtes de la barrière
        builder.HasIndex(i => i.BarriereId)
            .HasDatabaseName("IX_ItinerairesPrevu_Barriere");

        // Index unique pour éviter les doublons d'ordre par bon
        builder.HasIndex(i => new { i.BonId, i.OrdrePassage })
            .IsUnique()
            .HasDatabaseName("IX_ItinerairesPrevu_Bon_Ordre");
    }
}
