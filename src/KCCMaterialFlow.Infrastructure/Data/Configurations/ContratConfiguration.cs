using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Contrat (table ref.Contrats)
/// </summary>
public class ContratConfiguration : IEntityTypeConfiguration<Contrat>
{
    public void Configure(EntityTypeBuilder<Contrat> builder)
    {
        builder.ToTable("T_Contrats", "dbo");

        builder.HasKey(c => c.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(c => c.Id).HasColumnName("Id");

        builder.Property(c => c.PoNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ContratDescription)
            .HasMaxLength(500);

        builder.Property(c => c.CompagnieId)
            .IsRequired();

        builder.HasOne(c => c.Compagnie)
            .WithMany(comp => comp.Contrats)
            .HasForeignKey(c => c.CompagnieId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PoNumber);
        builder.HasIndex(c => c.CompagnieId);
    }
}
