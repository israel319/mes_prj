using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité CategorieSortie (table ref.CategoriesSortie)
/// </summary>
public class CategorieSortieConfiguration : IEntityTypeConfiguration<CategorieSortie>
{
    public void Configure(EntityTypeBuilder<CategorieSortie> builder)
    {
        builder.ToTable("T_CategoriesSortie", "dbo");

        builder.HasKey(c => c.IdCategorieSortie);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(c => c.IdCategorieSortie).HasColumnName("Id");

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Code)
            .HasMaxLength(10);

        builder.Property(c => c.Description)
            .HasMaxLength(200);

        builder.Property(c => c.TypeEntite)
            .HasMaxLength(50);

        builder.HasMany(c => c.Raisons)
            .WithOne(r => r.Categorie)
            .HasForeignKey(r => r.CategorieId);
    }
}
