using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonSortieInterne.
/// Configure les propriétés spécifiques aux transferts internes.
/// </summary>
public class BonSortieInterneConfiguration : IEntityTypeConfiguration<BonSortieInterne>
{
    public void Configure(EntityTypeBuilder<BonSortieInterne> builder)
    {
        // Propriétés spécifiques à BonSortieInterne
        builder.Property(b => b.BonEntreeAssocieId)
            .HasColumnName("BonEntreeAssocieId");

        builder.Property(b => b.DescriptionMateriel)
            .HasColumnName("DescriptionMaterielInterne")
            .HasMaxLength(200);

        builder.Property(b => b.DepartementOrigine)
            .HasColumnName("DepartementOrigine")
            .HasMaxLength(100);

        builder.Property(b => b.FonctionReceveur)
            .HasColumnName("FonctionReceveur")
            .HasMaxLength(150);

        builder.Property(b => b.EmailReceveur)
            .HasColumnName("EmailReceveur")
            .HasMaxLength(200);

        builder.Property(b => b.LocalisationDestination)
            .HasColumnName("LocalisationDestination")
            .HasMaxLength(200);

        builder.Property(b => b.DateTransfertPrevue)
            .HasColumnName("DateTransfertPrevue");

        builder.Property(b => b.DateTransfertEffective)
            .HasColumnName("DateTransfertEffective");

        // Index sur le département origine
        builder.HasIndex(b => b.DepartementOrigine)
            .HasDatabaseName("IX_BonsSortie_DeptOrigine");
    }
}
