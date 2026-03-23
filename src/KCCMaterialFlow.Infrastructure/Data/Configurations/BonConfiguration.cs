using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Module.BonEntree.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Bon (classe de base).
/// Utilise la stratégie TPH (Table Per Hierarchy) pour l'héritage.
/// Simplifié selon le diagramme de classe.
/// </summary>
public class BonConfiguration : IEntityTypeConfiguration<Bon>
{
    public void Configure(EntityTypeBuilder<Bon> builder)
    {
        // Table unique pour toute la hiérarchie (TPH)
        builder.ToTable("T_Bons", "dbo");

        builder.HasKey(b => b.IdBon);

        // Discriminateur pour TPH
        builder.HasDiscriminator<string>("TypeDiscriminator")
            .HasValue<BonEntree>("BonEntree");

        builder.Property(b => b.IdBon)
            .HasColumnName("IdBon")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.NumeroReference)
            .HasColumnName("NumeroReference")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.DateCreation)
            .HasColumnName("DateCreation")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(b => b.DateExpiration)
            .HasColumnName("DateExpiration")
            .IsRequired();

        builder.Property(b => b.StatutActuel)
            .HasColumnName("StatutActuel")
            .HasMaxLength(30)
            .HasDefaultValue("Draft");

        builder.Property(b => b.Provenance)
            .HasColumnName("Provenance")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Destination)
            .HasColumnName("Destination")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000);

        builder.Property(b => b.Quantite)
            .HasColumnName("Quantite")
            .HasDefaultValue(0);

        // Relations - Navigation vers les collections
        builder.HasMany(b => b.Materiels)
            .WithOne(m => m.Bon)
            .HasForeignKey(m => m.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Approbations)
            .WithOne(a => a.Bon)
            .HasForeignKey(a => a.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.ItinerairesPrevu)
            .WithOne(i => i.Bon)
            .HasForeignKey(i => i.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index unique sur le numéro de référence
        builder.HasIndex(b => b.NumeroReference)
            .IsUnique()
            .HasDatabaseName("IX_Bons_NumeroReference");

        // Index sur le statut pour les requêtes de workflow
        builder.HasIndex(b => b.StatutActuel)
            .HasDatabaseName("IX_Bons_Statut");

        // Index sur la date de création pour le tri
        builder.HasIndex(b => b.DateCreation)
            .HasDatabaseName("IX_Bons_DateCreation");
    }
}
