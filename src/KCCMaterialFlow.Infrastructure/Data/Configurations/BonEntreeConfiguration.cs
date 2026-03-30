using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonEntree (Aggregate Root).
/// Mappage complet vers la table T_Bons.
/// Simplifié selon le formulaire SEC-FM-141(B).
/// </summary>
public class BonEntreeConfiguration : IEntityTypeConfiguration<BonEntree>
{
    public void Configure(EntityTypeBuilder<BonEntree> builder)
    {
        // ===== Table et clé primaire =====
        builder.ToTable("T_Bons", "dbo");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("IdBon")
            .ValueGeneratedOnAdd();

        // ===== Propriétés communes (ex-Bon) =====
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

        // ===== Propriétés spécifiques à BonEntree =====
        builder.Property(b => b.ContratId)
            .HasColumnName("ContratId");

        builder.Property(b => b.NumeroContrat)
            .HasColumnName("NumeroContrat")
            .HasMaxLength(50);

        builder.Property(b => b.NomDemandeur)
            .HasColumnName("NomDemandeur")
            .HasMaxLength(200);

        builder.Property(b => b.NomCompagnie)
            .HasColumnName("NomCompagnie")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.EmailContractant)
            .HasColumnName("EmailContractant")
            .HasMaxLength(200);

        builder.Property(b => b.SiteManager)
            .HasColumnName("SiteManager")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.HostDepartment)
            .HasColumnName("HostDepartment")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.ReasonOnSite)
            .HasColumnName("ReasonOnSite")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.NomEscorteur)
            .HasColumnName("NomEscorteur")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.FonctionEscorteur)
            .HasColumnName("FonctionEscorteur")
            .HasMaxLength(150);

        // ===== Propriétés Liaison Entrée-Sortie (BSM-031) =====
        builder.Property(b => b.EstVerrouillePourSortie)
            .HasColumnName("EstVerrouillePourSortie")
            .HasDefaultValue(false);

        builder.Property(b => b.DateVerrouillage)
            .HasColumnName("DateVerrouillage");

        builder.Property(b => b.BonSortieAssocieId)
            .HasColumnName("BonSortieAssocieId");

        builder.Property(b => b.BonSortieAssocieNumero)
            .HasColumnName("BonSortieAssocieNumero")
            .HasMaxLength(20);

        // ===== Relations =====
        builder.HasMany(b => b.Materiels)
            .WithOne()
            .HasForeignKey(m => m.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Approbations)
            .WithOne()
            .HasForeignKey(a => a.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.ItinerairesPrevu)
            .WithOne()
            .HasForeignKey(i => i.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Historiques)
            .WithOne()
            .HasForeignKey(h => h.BonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== Indexes =====
        builder.HasIndex(b => b.NumeroReference)
            .IsUnique()
            .HasDatabaseName("IX_Bons_NumeroReference");

        builder.HasIndex(b => b.StatutActuel)
            .HasDatabaseName("IX_Bons_Statut");

        builder.HasIndex(b => b.DateCreation)
            .HasDatabaseName("IX_Bons_DateCreation");

        builder.HasIndex(b => b.NomCompagnie)
            .HasDatabaseName("IX_Bons_NomCompagnie");

        builder.HasIndex(b => b.HostDepartment)
            .HasDatabaseName("IX_Bons_HostDepartment");

        // BSM-031: Index pour trouver rapidement les BEM liés à un BSM
        builder.HasIndex(b => b.BonSortieAssocieId)
            .HasDatabaseName("IX_Bons_BonSortieAssocieId")
            .HasFilter("[BonSortieAssocieId] IS NOT NULL");

        // ===== Propriétés QR Code =====
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

        // ===== Ignorer les propriétés non mappées =====
        builder.Ignore(b => b.Statut);
        builder.Ignore(b => b.DomainEvents);
    }
}
