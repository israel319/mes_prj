using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité MaterielSortie.
/// </summary>
public class MaterielSortieConfiguration : IEntityTypeConfiguration<MaterielSortie>
{
    public void Configure(EntityTypeBuilder<MaterielSortie> builder)
    {
        builder.ToTable("T_MaterielsSortie", "dbo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("IdMateriel")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BonId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(m => m.CodeProduitSerial)
            .HasColumnName("CodeProduitSerial")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Designation)
            .HasColumnName("Designation")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(m => m.Quantite)
            .HasColumnName("Quantite")
            .HasPrecision(18, 4)
            .HasDefaultValue(1m);

        // Propriétés de liaison BEM
        builder.Property(m => m.MaterielEntreeId)
            .HasColumnName("MaterielEntreeId");

        builder.Property(m => m.BonEntreeId)
            .HasColumnName("BonEntreeId");

        builder.Property(m => m.BonEntreeNumero)
            .HasColumnName("BonEntreeNumero")
            .HasMaxLength(20);

        builder.Property(m => m.QuantiteInitialeBem)
            .HasColumnName("QuantiteInitialeBem")
            .HasPrecision(18, 4);

        builder.Property(m => m.QuantiteDisponible)
            .HasColumnName("QuantiteDisponible")
            .HasPrecision(18, 4);

        builder.Property(m => m.Observations)
            .HasColumnName("Observations")
            .HasMaxLength(500);

        // Index sur le bon de sortie
        builder.HasIndex(m => m.BonId)
            .HasDatabaseName("IX_MaterielsSortie_BonSortieId");

        // Index sur le code produit
        builder.HasIndex(m => m.CodeProduitSerial)
            .HasDatabaseName("IX_MaterielsSortie_CodeProduit");

        // Index sur le matériel source BEM
        builder.HasIndex(m => m.MaterielEntreeId)
            .HasDatabaseName("IX_MaterielsSortie_MaterielEntreeId");
    }
}
