using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité RaisonSortie (table ref.RaisonsSortie)
/// </summary>
public class RaisonSortieConfiguration : IEntityTypeConfiguration<RaisonSortie>
{
    public void Configure(EntityTypeBuilder<RaisonSortie> builder)
    {
        builder.ToTable("T_RaisonsSortie", "dbo");

        builder.HasKey(r => r.IdRaisonSortie);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(r => r.IdRaisonSortie).HasColumnName("Id");

        builder.Property(r => r.CategorieId)
            .IsRequired();

        builder.Property(r => r.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Code)
            .HasMaxLength(20);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.TypeApprobateurSpecial)
            .HasMaxLength(50);

        builder.Property(r => r.TypeMaterielDefaut)
            .HasColumnName("TypeMaterielDefaut")
            .HasConversion<int?>();

        builder.Property(r => r.Icone)
            .HasMaxLength(50);

        builder.Property(r => r.Couleur)
            .HasMaxLength(20);

        builder.HasOne(r => r.Categorie)
            .WithMany(c => c.Raisons)
            .HasForeignKey(r => r.CategorieId);
    }
}
