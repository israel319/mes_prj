using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Barriere
/// </summary>
public class BarriereConfiguration : IEntityTypeConfiguration<Barriere>
{
    public void Configure(EntityTypeBuilder<Barriere> builder)
    {
        builder.ToTable("T_Barrieres", "dbo");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("IdBarriere")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.CodeBarriere)
            .HasColumnName("CodeBarriere")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.NomBarriere)
            .HasColumnName("NomBarriere")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Localisation)
            .HasColumnName("Localisation")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(b => b.TypeBarriere)
            .HasColumnName("TypeBarriere")
            .HasMaxLength(50)
            .HasDefaultValue("Mixte");

        builder.Property(b => b.EstActive)
            .HasColumnName("EstActive")
            .HasDefaultValue(true);

        builder.Property(b => b.OrdreAffichage)
            .HasColumnName("OrdreAffichage")
            .HasDefaultValue(0);

        builder.Property(b => b.HorairesOuverture)
            .HasColumnName("HorairesOuverture")
            .HasMaxLength(100);

        builder.Property(b => b.Telephone)
            .HasColumnName("Telephone")
            .HasMaxLength(50);

        builder.Property(b => b.DateCreation)
            .HasColumnName("DateCreation")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(b => b.DateModification)
            .HasColumnName("DateModification");

        // Index unique sur le code barrière
        builder.HasIndex(b => b.CodeBarriere)
            .IsUnique()
            .HasDatabaseName("IX_Barrieres_Code");

        // Index sur la localisation pour les recherches
        builder.HasIndex(b => b.Localisation)
            .HasDatabaseName("IX_Barrieres_Localisation");

        // Index sur EstActive
        builder.HasIndex(b => b.EstActive)
            .HasDatabaseName("IX_Barrieres_EstActive");

        // Index sur le type de barrière
        builder.HasIndex(b => b.TypeBarriere)
            .HasDatabaseName("IX_Barrieres_Type");
    }
}
