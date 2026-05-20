using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonSortie (classe de base des sorties).
/// Utilise la stratégie TPH (Table Per Hierarchy) pour l'héritage.
/// Schéma: bsm
/// </summary>
public class BonSortieConfiguration : IEntityTypeConfiguration<BonSortie>
{
    public void Configure(EntityTypeBuilder<BonSortie> builder)
    {
        // Table unique pour toute la hiérarchie des bons de sortie (TPH)
        builder.ToTable("T_BonsSortie", "dbo");

        builder.HasKey(b => b.Id);

        // Discriminateur pour TPH - distingue les types de sortie
        builder.HasDiscriminator<string>("TypeSortie")
            .HasValue<BonSortieExterne>("Externe")
            .HasValue<BonSortieInterne>("Interne")
            .HasValue<Pret>("Pret");

        builder.Property(b => b.Id)
            .HasColumnName("IdBon")
            .ValueGeneratedOnAdd();

        // Propriétés héritées de Bon
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

        // Propriétés spécifiques à BonSortie
        builder.Property(b => b.NomDemandeur)
            .HasColumnName("NomDemandeur")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.FonctionDemandeur)
            .HasColumnName("FonctionDemandeur")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.DepartementDemandeur)
            .HasColumnName("DepartementDemandeur")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.MotifSortie)
            .HasColumnName("MotifSortie")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(b => b.EstDefinitif)
            .HasColumnName("EstDefinitif")
            .HasDefaultValue(true);

        // ===== Site + RequestedFor (v2) =====
        builder.Property(b => b.SiteId).HasColumnName("SiteId");
        builder.HasOne(b => b.Site)
            .WithMany()
            .HasForeignKey(b => b.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== TypeMaterielSortie (workflow routing) =====
        builder.Property(b => b.TypeMaterielSortieId).HasColumnName("TypeMaterielSortieId");
        builder.HasOne(b => b.TypeMaterielSortie)
            .WithMany()
            .HasForeignKey(b => b.TypeMaterielSortieId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(b => b.RequestedForEmployeeCode)
            .HasColumnName("RequestedForEmployeeCode")
            .HasMaxLength(50);
        builder.Property(b => b.RequestedForDisplay)
            .HasColumnName("RequestedForDisplay")
            .HasMaxLength(200);
        builder.Property(b => b.RequestedForDepartement)
            .HasColumnName("RequestedForDepartement")
            .HasMaxLength(200);
        builder.Property(b => b.SiteManager)
            .HasColumnName("SiteManager")
            .HasMaxLength(200);

        // ===== Propriétés QR Code (BSM-030) =====
        builder.Property(b => b.QRCodeData)
            .HasColumnName("QRCodeData")
            .HasMaxLength(500);

        builder.Property(b => b.QRCodeBase64)
            .HasColumnName("QRCodeBase64")
            .HasColumnType("nvarchar(max)");

        builder.Property(b => b.QRCodeHash)
            .HasColumnName("QRCodeHash")
            .HasMaxLength(128);

        builder.Property(b => b.DateGenerationQR)
            .HasColumnName("DateGenerationQR");

        // Relations - Navigation vers les collections propres au module BonSortie
        builder.HasMany(b => b.Materiels)
            .WithOne()
            .HasForeignKey(m => m.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Approbations)
            .WithOne()
            .HasForeignKey(a => a.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Itineraires)
            .WithOne()
            .HasForeignKey(i => i.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Historiques)
            .WithOne()
            .HasForeignKey(h => h.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== Ignorer les propriétés non mappées =====
        builder.Ignore(b => b.Statut);
        builder.Ignore(b => b.DomainEvents);

        // Index unique sur le numéro de référence
        builder.HasIndex(b => b.NumeroReference)
            .IsUnique()
            .HasDatabaseName("IX_BonsSortie_NumeroReference");

        // Index sur le statut pour les requêtes de workflow
        builder.HasIndex(b => b.StatutActuel)
            .HasDatabaseName("IX_BonsSortie_Statut");

        // Index sur la date de création pour le tri
        builder.HasIndex(b => b.DateCreation)
            .HasDatabaseName("IX_BonsSortie_DateCreation");

        // Index sur le département demandeur
        builder.HasIndex(b => b.DepartementDemandeur)
            .HasDatabaseName("IX_BonsSortie_Departement");
    }
}
