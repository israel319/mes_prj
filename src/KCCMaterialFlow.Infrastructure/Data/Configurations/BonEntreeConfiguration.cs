using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.BonEntree.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonEntree.
/// Hérite de Bon via TPH (discriminateur dans BonConfiguration).
/// Simplifié selon le formulaire SEC-FM-141(B).
/// </summary>
public class BonEntreeConfiguration : IEntityTypeConfiguration<BonEntree>
{
    public void Configure(EntityTypeBuilder<BonEntree> builder)
    {
        // Propriétés spécifiques à BonEntree (8 champs selon SEC-FM-141(B))
        builder.Property(b => b.ContratId)
            .HasColumnName("ContratId");

        builder.Property(b => b.NumeroContrat)
            .HasColumnName("NumeroContrat")
            .HasMaxLength(50);

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

        // Index sur la compagnie pour les recherches
        builder.HasIndex(b => b.NomCompagnie)
            .HasDatabaseName("IX_Bons_NomCompagnie");

        // Index sur le département hôte
        builder.HasIndex(b => b.HostDepartment)
            .HasDatabaseName("IX_Bons_HostDepartment");

        // BSM-031: Index pour trouver rapidement les BEM liés à un BSM
        builder.HasIndex(b => b.BonSortieAssocieId)
            .HasDatabaseName("IX_Bons_BonSortieAssocieId")
            .HasFilter("[BonSortieAssocieId] IS NOT NULL");
    }
}
