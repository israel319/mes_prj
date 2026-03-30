using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ItineraireSortie.
/// </summary>
public class ItineraireSortieConfiguration : IEntityTypeConfiguration<ItineraireSortie>
{
    public void Configure(EntityTypeBuilder<ItineraireSortie> builder)
    {
        builder.ToTable("T_ItinerairesSortie", "dbo");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("IdItineraire")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.BonId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(i => i.BarriereId)
            .HasColumnName("BarriereId")
            .IsRequired();

        builder.Property(i => i.OrdrePassage)
            .HasColumnName("OrdrePassage");

        // Index sur le bon de sortie
        builder.HasIndex(i => i.BonId)
            .HasDatabaseName("IX_ItinerairesSortie_BonSortieId");

        // Index sur la barrière
        builder.HasIndex(i => i.BarriereId)
            .HasDatabaseName("IX_ItinerairesSortie_BarriereId");

        // Index composé pour l'ordre de passage
        builder.HasIndex(i => new { i.BonId, i.OrdrePassage })
            .HasDatabaseName("IX_ItinerairesSortie_Ordre");
    }
}
