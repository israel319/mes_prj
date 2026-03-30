using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonSortieExterne.
/// Configure les propriétés spécifiques et la relation optionnelle vers BonEntree.
/// </summary>
public class BonSortieExterneConfiguration : IEntityTypeConfiguration<BonSortieExterne>
{
    public void Configure(EntityTypeBuilder<BonSortieExterne> builder)
    {
        // Propriétés spécifiques à BonSortieExterne
        builder.Property(b => b.BonEntreeAssocieId)
            .HasColumnName("BonEntreeAssocieId");

        builder.Property(b => b.TypeMateriel)
            .HasColumnName("TypeMateriel")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.NomDestinataire)
            .HasColumnName("NomDestinataire")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.AdresseDestination)
            .HasColumnName("AdresseDestination")
            .HasMaxLength(500);

        builder.Property(b => b.NumeroVehicule)
            .HasColumnName("NumeroVehicule")
            .HasMaxLength(50);

        builder.Property(b => b.NomChauffeur)
            .HasColumnName("NomChauffeur")
            .HasMaxLength(200);

        builder.Property(b => b.TelephoneChauffeur)
            .HasColumnName("TelephoneChauffeur")
            .HasMaxLength(50);

        // Relation 0..1 vers BonEntree (FK optionnelle)
        builder.HasOne<BonEntree>()
            .WithMany()
            .HasForeignKey(b => b.BonEntreeAssocieId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Index sur le BonEntree associé pour les recherches
        builder.HasIndex(b => b.BonEntreeAssocieId)
            .HasDatabaseName("IX_BonsSortie_BonEntreeAssocie");

        // Index sur le type de matériel
        builder.HasIndex(b => b.TypeMateriel)
            .HasDatabaseName("IX_BonsSortie_TypeMateriel");

        // Index sur le destinataire
        builder.HasIndex(b => b.NomDestinataire)
            .HasDatabaseName("IX_BonsSortie_Destinataire");
    }
}
