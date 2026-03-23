using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.BonEntree.Entities;

using KCCMaterialFlow.Module.BonSortie.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ItineraireSortie.
/// </summary>
public class ItineraireSortieConfiguration : IEntityTypeConfiguration<ItineraireSortie>
{
    public void Configure(EntityTypeBuilder<ItineraireSortie> builder)
    {
        builder.ToTable("T_ItinerairesSortie", "dbo");

        builder.HasKey(i => i.IdItineraire);

        builder.Property(i => i.IdItineraire)
            .HasColumnName("IdItineraire")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.BonSortieId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(i => i.BarriereId)
            .HasColumnName("BarriereId")
            .IsRequired();

        builder.Property(i => i.OrdrePassage)
            .HasColumnName("OrdrePassage");

        builder.Property(i => i.DatePassagePrevue)
            .HasColumnName("DatePassagePrevue");

        builder.Property(i => i.DatePassageEffective)
            .HasColumnName("DatePassageEffective");

        builder.Property(i => i.StatutPassage)
            .HasColumnName("StatutPassage")
            .HasMaxLength(50)
            .HasDefaultValue("Prévu");

        builder.Property(i => i.Observations)
            .HasColumnName("Observations")
            .HasMaxLength(500);

        // Index sur le bon de sortie
        builder.HasIndex(i => i.BonSortieId)
            .HasDatabaseName("IX_ItinerairesSortie_BonSortieId");

        // Index sur la barrière
        builder.HasIndex(i => i.BarriereId)
            .HasDatabaseName("IX_ItinerairesSortie_BarriereId");

        // Index composé pour l'ordre de passage
        builder.HasIndex(i => new { i.BonSortieId, i.OrdrePassage })
            .HasDatabaseName("IX_ItinerairesSortie_Ordre");
    }
}
