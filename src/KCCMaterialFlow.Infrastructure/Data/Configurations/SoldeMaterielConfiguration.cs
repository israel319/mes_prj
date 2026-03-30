using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité SoldeMateriel (table bem.SoldesMateriels)
/// </summary>
public class SoldeMaterielConfiguration : IEntityTypeConfiguration<SoldeMateriel>
{
    public void Configure(EntityTypeBuilder<SoldeMateriel> builder)
    {
        builder.ToTable("T_SoldesMateriels", "dbo");

        builder.HasKey(s => s.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(s => s.Id).HasColumnName("Id");

        builder.Property(s => s.MaterielEntreeId)
            .IsRequired();

        builder.Property(s => s.BonEntreeId)
            .IsRequired();

        builder.Property(s => s.CodeProduitSerial)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Designation)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(s => s.QuantiteInitiale)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.QuantiteSortie)
            .HasColumnType("decimal(18,4)");

        builder.Property(s => s.DernierBsmNumero)
            .HasMaxLength(20);

        // Propriétés calculées non-mappées
        builder.Ignore(s => s.QuantiteRestante);
        builder.Ignore(s => s.EstEpuise);
        builder.Ignore(s => s.EstPartiel);
    }
}
