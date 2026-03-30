using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Materiel.
/// Simplifié selon le diagramme de classe (5 champs).
/// </summary>
public class MaterielConfiguration : IEntityTypeConfiguration<Materiel>
{
    public void Configure(EntityTypeBuilder<Materiel> builder)
    {
        builder.ToTable("T_Materiels", "dbo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("IdMateriel")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BonId)
            .HasColumnName("BonId")
            .IsRequired();

        builder.Property(m => m.CodeProduitSerial)
            .HasColumnName("CodeProduitSerial")
            .HasMaxLength(100);

        builder.Property(m => m.Designation)
            .HasColumnName("Designation")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(m => m.Quantite)
            .HasColumnName("Quantite")
            .HasPrecision(18, 2)
            .HasDefaultValue(1m);

        // Relation avec le Bon (définie dans BonConfiguration)
        // Ne pas redéfinir ici pour éviter les conflits

        // Index sur le bon pour récupérer les matériels
        builder.HasIndex(m => m.BonId)
            .HasDatabaseName("IX_Materiels_Bon");

        // Index sur le code produit/serial
        builder.HasIndex(m => m.CodeProduitSerial)
            .HasDatabaseName("IX_Materiels_CodeSerial");
    }
}
