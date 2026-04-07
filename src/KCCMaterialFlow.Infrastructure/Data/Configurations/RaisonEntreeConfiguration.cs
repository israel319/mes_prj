using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour T_RaisonsEntree (motifs d'entrée structurés).
/// </summary>
public class RaisonEntreeConfiguration : IEntityTypeConfiguration<RaisonEntree>
{
    public void Configure(EntityTypeBuilder<RaisonEntree> builder)
    {
        builder.ToTable("T_RaisonsEntree", "dbo");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nom)
            .HasColumnName("Nom")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Code)
            .HasColumnName("Code")
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(r => r.EstActif)
            .HasColumnName("EstActif")
            .HasDefaultValue(true);

        builder.Property(r => r.OrdreAffichage)
            .HasColumnName("OrdreAffichage")
            .HasDefaultValue(0);

        builder.Property(r => r.Icone)
            .HasColumnName("Icone")
            .HasMaxLength(50);

        builder.Property(r => r.Couleur)
            .HasColumnName("Couleur")
            .HasMaxLength(20);

        // Index unique sur le code
        builder.HasIndex(r => r.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL")
            .HasDatabaseName("IX_RaisonsEntree_Code");
    }
}
